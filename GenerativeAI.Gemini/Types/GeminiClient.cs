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
    /// <summary>
    /// Provides a concrete implementation of <see cref="IGeminiClient"/> for interacting with the Gemini API.
    /// </summary>
    internal sealed class GeminiClient(IOptions<GeminiPromptOptions> options) : IGeminiClient
    {
        /// <summary>
        /// Represents the configuration payload sent to the Gemini API.
        /// </summary>
        /// <param name="Contents">The collection of content parts that make up the prompt.</param>
        /// <param name="GenerationConfig">The configuration for response generation.</param>
        private record PromptConfig(ICollection<Content> Contents, GenerationConfig GenerationConfig);

        /// <summary>
        /// The JSON serializer settings used for all Gemini API requests and responses.
        /// </summary>
        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            ContractResolver = new GeminiContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = [new StringEnumConverter()]
        };

        /// <summary>
        /// Gets or sets the default configuration options used for Gemini prompts and API requests.
        /// </summary>
        public GeminiPromptOptions DefaultPromptOptions { get; set; } = options.Value;

        /// <summary>
        /// Creates a new <see cref="IGeminiPrompt"/> instance using the specified options or the default options if none are provided.
        /// </summary>
        /// <param name="defaultOptions">Optional. The prompt options to use for the new prompt. If <c>null</c>, <see cref="DefaultPromptOptions"/> is used.</param>
        /// <returns>A new <see cref="IGeminiPrompt"/> instance.</returns>
        public IGeminiPrompt CreatePrompt(GeminiPromptOptions? defaultOptions = null) => new GeminiPrompt((defaultOptions ?? DefaultPromptOptions) with { });

        /// <summary>
        /// Executes the Gemini model using the specified prompt and returns a response containing plain text data.
        /// </summary>
        /// <param name="prompt">The prompt to send to the Gemini API.</param>
        /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task representing the asynchronous operation, with a result of <see cref="IGeminiResponse{String}"/> containing the model's response as a string.
        /// </returns>
        public async Task<IGeminiResponse<string>> ExecuteModelAsync(IGeminiPrompt prompt, CancellationToken cancellationToken = default) => await ExecuteModelAsync<string>(prompt, cancellationToken);

        /// <summary>
        /// Executes the Gemini model using the specified prompt and returns a response containing data deserialized to the specified schema type.
        /// </summary>
        /// <typeparam name="TSchema">The type to which the response data should be deserialized.</typeparam>
        /// <param name="prompt">The prompt to send to the Gemini API.</param>
        /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task representing the asynchronous operation, with a result of <see cref="IGeminiResponse{TSchema}"/> containing the model's response as the specified type.
        /// </returns>
        public async Task<IGeminiResponse<TSchema>> ExecuteModelAsync<TSchema>(IGeminiPrompt prompt, CancellationToken cancellationToken = default)
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
                        clonedPrompt.AddText($"Continue. follow this schema: {JsonConvert.SerializeObject(schema)}");
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
