using GenerativeAI.Gemini.Contracts;
using GenerativeAI.Gemini.Enums;
using GenerativeAI.Gemini.Models;
using GenerativeAI.Gemini.Serializer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net.Mime;
using System.Text;

namespace GenerativeAI.Gemini.Types
{
    /// <inheritdoc cref="IGeminiClient" />
    /// <param name="options">
    /// The configured <see cref="GeminiPromptOptions"/> bound by DI via <see cref="IOptions{TOptions}"/>.
    /// These values seed <see cref="DefaultPromptOptions"/>.
    /// </param>
    public sealed class GeminiClient(IOptions<GeminiPromptOptions> options) : IGeminiClient
    {
        /// <summary>
        /// Represents the configuration payload sent to the Gemini API.
        /// </summary>
        /// <param name="Contents">The collection of content parts that make up the prompt.</param>
        /// <param name="GenerationConfig">The configuration for response generation.</param>
        private sealed record PromptConfig(ICollection<Content> Contents, GenerationConfig GenerationConfig);

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

        /// <inheritdoc cref="IGeminiClient.ExecuteAsync(IGeminiPrompt, CancellationToken)" />
        public async Task<IGeminiResponse<string>> ExecuteAsync(IGeminiPrompt prompt, CancellationToken cancellationToken = default) => await CallGeminiAsync<string>(prompt, cancellationToken);

        /// <inheritdoc cref="IGeminiClient.ExecuteAsync{TSchema}(IGeminiPrompt, CancellationToken)" />
        public async Task<IGeminiResponse<TSchema>> ExecuteAsync<TSchema>(IGeminiPrompt prompt, CancellationToken cancellationToken = default) => await CallGeminiAsync<TSchema>(prompt, cancellationToken);

        /// <summary>
        /// Executes the Gemini API for the given prompt and materializes a typed response.
        /// Handles multi-part (paged) completions by continuing when the model stops at MaxTokens,
        /// accumulating all response parts and returning a consolidated <see cref="IGeminiResponse{TSchema}"/>.
        /// </summary>
        /// <typeparam name="TSchema">
        /// The target type for the model output.
        /// Use <see cref="string"/> to receive raw concatenated text; otherwise the text is JSON-deserialized into <typeparamref name="TSchema"/>.
        /// A JSON schema derived from <typeparamref name="TSchema"/> is provided to the model to encourage structured output.
        /// </typeparam>
        /// <param name="prompt">
        /// The prompt to execute. A clone is created internally so the original is not mutated.
        /// The <see cref="IGeminiResponse{TSchema}.Prompt"/> in the result contains the augmented prompt (with model candidates appended).
        /// </param>
        /// <param name="cancellationToken">A token to observe for cancellation of the underlying HTTP requests.</param>
        /// <returns>
        /// An <see cref="IGeminiResponse{TSchema}"/> containing the deserialized data (if successful).
        /// </returns>
        /// <remarks>
        /// - Resolves HttpClient, ApiKey, and Model from the prompt options or from <see cref="DefaultPromptOptions"/>.<br/>
        /// - If the last candidate finishes with <c>MaxTokens</c>, the method appends a "Continue" instruction and retries without the response schema to complete the output.<br/>
        /// - All response parts are accumulated to preserve usage metadata and full text.
        /// </remarks>
        private async Task<IGeminiResponse<TSchema>> CallGeminiAsync<TSchema>(IGeminiPrompt prompt, CancellationToken cancellationToken = default)
        {
            try
            {
                var clonedPrompt = prompt.Clone();
                var schema = typeof(TSchema) != typeof(string) ? GeminiSchemaBuilder.GetSchema(typeof(TSchema)) : null;
                var httpClient = clonedPrompt.PromptOptions.HttpClient ?? DefaultPromptOptions.HttpClient ?? throw new InvalidOperationException("HttpClient not found.");
                var apiKey = clonedPrompt.PromptOptions.ApiKey ?? DefaultPromptOptions.ApiKey ?? throw new InvalidOperationException("ApiKey not found.");
                var model = clonedPrompt.PromptOptions.Model ?? DefaultPromptOptions.Model ?? throw new InvalidOperationException("Model version not found.", new());
                var generationConfig = new GenerationConfig(clonedPrompt.PromptOptions, schema);

                GeminiResponseData? serializedResponse;
                var responseParts = new List<GeminiResponseData>();
                do
                {
                    var response = await CallGeminiAsync(clonedPrompt.Contents, generationConfig, model, apiKey, httpClient, cancellationToken);
                    serializedResponse = JsonConvert.DeserializeObject<GeminiResponseData>(response, JsonSerializerSettings) ?? throw new InvalidOperationException(response);
                    if (serializedResponse is not null) responseParts.Add(serializedResponse);
                    clonedPrompt.Contents = [.. clonedPrompt.Contents, .. serializedResponse?.Candidates.Select(c => c.Content) ?? []];

                    if (serializedResponse?.Candidates.LastOrDefault()?.FinishReason is FinishReason.MaxTokens)
                    {
                        clonedPrompt.AddText($"Continue. follow this schema: {JsonConvert.SerializeObject(schema, JsonSerializerSettings)}");
                        generationConfig = generationConfig with { ResponseSchema = null };
                    }
                } while (serializedResponse?.Candidates.LastOrDefault()?.FinishReason is FinishReason.MaxTokens);

                return new GeminiResponse<TSchema>(responseParts, clonedPrompt, JsonSerializerSettings);
            }
            catch (Exception ex)
            {
                return new GeminiResponse<TSchema>(prompt, ex);
            }
        }

        /// <summary>
        /// Sends a request to the Gemini API with the specified prompt contents and generation configuration.
        /// </summary>
        /// <param name="contents">The collection of content parts that make up the prompt.</param>
        /// <param name="generationConfig">The configuration for response generation.</param>
        /// <param name="model">The Gemini model version to use.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="httpClient">The HTTP client used to send the request.</param>
        /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
        /// <returns>The raw JSON response from the Gemini API as a string.</returns>
        /// <exception cref="BadHttpRequestException">Thrown if the HTTP response indicates a failure.</exception>
        private static async Task<string> CallGeminiAsync(ICollection<Content> contents, GenerationConfig generationConfig, string model, string apiKey, HttpClient httpClient, CancellationToken cancellationToken = default)
        {
            var uri = new Uri(@$"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}");

            using var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new PromptConfig(contents, generationConfig), JsonSerializerSettings), Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            using var response = await httpClient.SendAsync(request, cancellationToken);
            
            var responseText = await response.Content.ReadAsStringAsync(cancellationToken);
            return response.IsSuccessStatusCode ? responseText : throw new BadHttpRequestException(responseText, (int)response.StatusCode);
        }
    }
}
