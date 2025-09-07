using GenerativeAI.Gemini.Contracts;
using GenerativeAI.Gemini.Enums;
using GenerativeAI.Gemini.Models;
using GenerativeAI.Gemini.Serializer;
using GenerativeAI.Gemini.Types.Prompt;
using GenerativeAI.Gemini.Types.PromptConfig;
using GenerativeAI.Gemini.Types.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net.Mime;
using System.Text;

namespace GenerativeAI.Gemini.Types
{
    /// <summary>
    /// Provides a client for interacting with Google's Gemini API. Implements <see cref="IGeminiClient"/>.
    /// </summary>
    /// <param name="options">
    /// The configured <see cref="GeminiPromptOptions"/> bound by DI via <see cref="IOptions{TOptions}"/>.
    /// These values seed <see cref="DefaultPromptOptions"/>.
    /// </param>
    public sealed class GeminiClient(IOptions<GeminiPromptOptions> options) : IGeminiClient
    {
        /// <summary>
        /// Encapsulates all the configuration and data required to make a request to the Gemini API.
        /// </summary>
        /// <param name="Payload">The request payload.</param>
        /// <param name="Model">The Gemini model version to use for the request.</param>
        /// <param name="ApiKey">The API key for authentication with the Gemini API.</param>
        /// <param name="HttpClient">The HTTP client used to send the request.</param>
        private sealed record RequestConfig(PromptPayload Payload, string Model, string ApiKey, HttpClient HttpClient);

        /// <summary>
        /// Represents the configuration payload sent to the Gemini API.
        /// </summary>
        /// <param name="Contents">The collection of content parts that make up the prompt.</param>
        /// <param name="GenerationConfig">The configuration for response generation.</param>
        /// <param name="Tools">The configuration for tools.</param>
        private sealed record PromptPayload(IEnumerable<Content> Contents, GenerationConfig GenerationConfig, IEnumerable<object> Tools);

        /// <summary>
        /// The JSON serializer settings used for all Gemini API requests and responses.
        /// </summary>
        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            ContractResolver = new GeminiContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = [new StringEnumConverter()]
        };

        /// <inheritdoc cref="IGeminiClient.DefaultPromptOptions" />
        public GeminiPromptOptions DefaultPromptOptions { get; set; } = options.Value;

        /// <summary>
        /// Initializes a new instance of <see cref="GeminiClient"/>.
        /// Seeds <see cref="DefaultPromptOptions"/> with the provided values.
        /// </summary>
        /// <param name="apiKey">The Gemini API key to use for authentication.</param>
        /// <param name="defaultModel">Optional. The default model identifier.</param>
        /// <param name="httpClient">
        /// Optional. The <see cref="HttpClient"/> to use for requests. If <c>null</c>, a new <see cref="HttpClient"/> is created.
        /// </param>
        public GeminiClient(string apiKey, string? defaultModel = null, HttpClient? httpClient = null) : this(Options.Create(new GeminiPromptOptions()))
        {
            DefaultPromptOptions.ApiKey = apiKey;
            DefaultPromptOptions.Model = defaultModel;
            DefaultPromptOptions.HttpClient = httpClient ?? new();
        }

        /// <param name="defaultModel">
        /// Optional. The default <see cref="GeminiModel"/> to use. This is mapped to the official model identifier.
        /// </param>
        /// <inheritdoc cref="GeminiClient(string, string?, HttpClient?)" />
#pragma warning disable CS1573
        public GeminiClient(string apiKey, GeminiModel? defaultModel = null, HttpClient? httpClient = null) : this(apiKey, defaultModel.HasValue ? GeminiPromptOptions.GetModelName(defaultModel.Value) : null, httpClient) { }
#pragma warning restore CS1573

        /// <inheritdoc cref="IGeminiClient.CreatePrompt(GeminiPromptOptions?)" />
        public IGeminiPrompt CreatePrompt(GeminiPromptOptions? defaultOptions = null) => new GeminiPrompt((defaultOptions ?? DefaultPromptOptions) with { });

        /// <inheritdoc cref="IGeminiClient.GenerateTextAsync(IGeminiPrompt, GeminiTextPromptConfig?, CancellationToken)" />
        public async Task<GeminiTextResponse> GenerateTextAsync(IGeminiPrompt prompt, GeminiTextPromptConfig? config = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var clonedPrompt = prompt.Clone();
                var response = await CallGeminiAsync(GetPayload(clonedPrompt, tools: config?.GetTools()), cancellationToken);
                var serializedResponse = JsonConvert.DeserializeObject<ResponseData>(response, JsonSerializerSettings) ?? throw new InvalidOperationException(response);
                return new(serializedResponse, clonedPrompt);
            }
            catch (Exception ex)
            {
                return new(prompt, ex);
            }
        }

        /// <inheritdoc cref="IGeminiClient.GenerateObjectAsync{TSchema}(IGeminiPrompt, CancellationToken)" />
        public async Task<GeminiStructuredResponse<TSchema>> GenerateObjectAsync<TSchema>(IGeminiPrompt prompt, CancellationToken cancellationToken = default)
        {
            try
            {
                var clonedPrompt = prompt.Clone();
                var schema = typeof(TSchema) != typeof(string) ? GeminiSchemaBuilder.GetSchema(typeof(TSchema)) : null;
                var requestConfig = GetPayload(clonedPrompt, schema);

                ResponseData? serializedResponse;
                var responseParts = new List<ResponseData>();
                do
                {
                    var response = await CallGeminiAsync(requestConfig, cancellationToken);
                    serializedResponse = JsonConvert.DeserializeObject<ResponseData>(response, JsonSerializerSettings) ?? throw new InvalidOperationException(response);
                    if (serializedResponse is not null)
                        responseParts.Add(serializedResponse);
                    clonedPrompt.Contents = [.. clonedPrompt.Contents, .. serializedResponse?.Candidates.Select(c => c.Content) ?? []];

                    if (serializedResponse?.Candidates.LastOrDefault()?.FinishReason is FinishReason.MaxTokens)
                    {
                        clonedPrompt.AddText($"Continue. follow this schema: {JsonConvert.SerializeObject(schema, JsonSerializerSettings)}");
                        requestConfig = GetPayload(clonedPrompt);
                    }
                } while (serializedResponse?.Candidates.LastOrDefault()?.FinishReason is FinishReason.MaxTokens);

                return new(responseParts, clonedPrompt, JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                return new(prompt, ex);
            }
        }

        /// <summary>
        /// Creates a request configuration object containing all necessary data for a Gemini API call.
        /// </summary>
        /// <param name="prompt">
        /// The prompt containing content and configuration options to be sent to the API.
        /// </param>
        /// <param name="schema">
        /// Optional. The response schema object that defines the expected structure of the model's response.
        /// </param>
        /// <param name="tools">
        /// Optional. Collection of tool configurations to enable additional capabilities.
        /// </param>
        /// <returns>
        /// A <see cref="RequestConfig"/> containing the complete configuration for making the API request.
        /// </returns>
        private RequestConfig GetPayload(IGeminiPrompt prompt, object? schema = null, IEnumerable<object>? tools = null)
        {
            var clonedPrompt = prompt.Clone();
            var httpClient = clonedPrompt.PromptOptions.HttpClient ?? DefaultPromptOptions.HttpClient ?? throw new InvalidOperationException("HttpClient not found.");
            var apiKey = clonedPrompt.PromptOptions.ApiKey ?? DefaultPromptOptions.ApiKey ?? throw new InvalidOperationException("ApiKey not found.");
            var model = clonedPrompt.PromptOptions.Model ?? DefaultPromptOptions.Model ?? throw new InvalidOperationException("Model version not found.", new());
            var generationConfig = new GenerationConfig(clonedPrompt.PromptOptions, schema);
            return new(new(clonedPrompt.Contents, generationConfig, tools ?? []), model, apiKey, httpClient);
        }

        /// <summary>
        /// Sends a request to the Gemini API using the provided request configuration.
        /// </summary>
        /// <param name="requestConfig">The complete request configuration containing.</param>
        /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
        /// <returns>The raw JSON response from the Gemini API as a string.</returns>
        /// <exception cref="BadHttpRequestException">Thrown if the HTTP response indicates a failure.</exception>
        private static async Task<string> CallGeminiAsync(RequestConfig requestConfig, CancellationToken cancellationToken = default)
        {
            var uri = new Uri(@$"https://generativelanguage.googleapis.com/v1beta/models/{requestConfig.Model}:generateContent?key={requestConfig.ApiKey}");

            using var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestConfig.Payload, JsonSerializerSettings), Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            using var response = await requestConfig.HttpClient.SendAsync(request, cancellationToken);
            var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            return response.IsSuccessStatusCode ? responseText : throw new BadHttpRequestException(responseText, (int)response.StatusCode);
        }
    }
}
