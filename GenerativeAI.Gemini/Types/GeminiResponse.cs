using GenerativeAI.Gemini.Contracts;
using GenerativeAI.Gemini.Models.Part;
using Newtonsoft.Json;

namespace GenerativeAI.Gemini.Types
{
    /// <inheritdoc cref="IGeminiResponse{TSchema}" />
    public sealed record GeminiResponse<TSchema> : IGeminiResponse<TSchema>
    {
        /// <inheritdoc cref="IGeminiResponse{TSchema}.Data" />
        public TSchema? Data { get; }

        /// <inheritdoc cref="IGeminiResponse{TSchema}.Prompt" />
        public IGeminiPrompt Prompt { get; }

        /// <inheritdoc cref="IGeminiResponse{TSchema}.ResponseParts" />
        public IReadOnlyCollection<GeminiResponseData> ResponseParts { get; } = [];

        /// <inheritdoc cref="IGeminiResponse{TSchema}.ErrorException" />
        public Exception? ErrorException { get; }

        /// <inheritdoc cref="IGeminiResponse{TSchema}.IsSuccessful" />
        public bool IsSuccessful => ErrorException is null && Data is not null;

        /// <summary>
        /// Constructs a <see cref="GeminiResponse{TSchema}"/> from a collection of response parts and the original prompt.
        /// Optionally accepts custom JSON serializer settings for deserialization.
        /// </summary>
        /// <param name="responseParts">The response parts returned by the Gemini API.</param>
        /// <param name="prompt">The original prompt sent to the API.</param>
        /// <param name="serializerSettings">Optional JSON serializer settings for deserialization.</param>
        public GeminiResponse(IReadOnlyCollection<GeminiResponseData> responseParts, IGeminiPrompt prompt, JsonSerializerSettings? serializerSettings = null)
        {
            Prompt = prompt;
            ResponseParts = responseParts;
            Data = GetResponseData(responseParts, serializerSettings);
        }

        /// <summary>
        /// Initializes a new <see cref="GeminiResponse{TSchema}"/> instance representing an error response.
        /// Sets the <paramref name="errorException"/> property and associates the response with the original prompt.
        /// The <c>Data</c> property will be <c>null</c> and <c>ResponseParts</c> will be empty.
        /// </summary>
        /// <param name="prompt">The original prompt sent to the Gemini API.</param>
        /// <param name="errorException">The exception that occurred during the response.</param>
        public GeminiResponse(IGeminiPrompt prompt, Exception errorException)
        {
            Prompt = prompt;
            ErrorException = errorException;
        }

        /// <summary>
        /// Extracts and deserializes the response data from the provided response parts.
        /// If <typeparamref name="TSchema"/> is <c>string</c>, returns the concatenated text content.
        /// Otherwise, deserializes the text content to <typeparamref name="TSchema"/> using the provided serializer settings.
        /// </summary>
        /// <param name="responseParts">The response parts to extract data from.</param>
        /// <param name="serializerSettings">Optional JSON serializer settings.</param>
        /// <returns>The deserialized response data.</returns>
        public static TSchema? GetResponseData(IReadOnlyCollection<GeminiResponseData> responseParts, JsonSerializerSettings? serializerSettings = null) => string.Join(string.Empty, responseParts.Select(response => response.Candidates.FirstOrDefault()?.Content.Parts.OfType<TextPart>().FirstOrDefault()?.Text)) switch
        {
            string text when typeof(TSchema) == typeof(string) => (TSchema)(object)text,
            string text => JsonConvert.DeserializeObject<TSchema>(text, serializerSettings),
            _ => default
        };
    }
}
