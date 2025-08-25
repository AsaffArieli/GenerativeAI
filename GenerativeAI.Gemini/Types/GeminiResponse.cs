using GenerativeAI.Gemini.Models.Part;
using Newtonsoft.Json;

namespace GenerativeAI.Gemini.Types
{
    /// <summary>
    /// Represents a response from a Gemini API prompt.
    /// </summary>
    /// <typeparam name="TSchema">The type to which the response data should be deserialized.</typeparam>
    /// <param name="Data">
    /// The deserialized response data of type <typeparamref name="TSchema"/>.
    /// This is constructed from the text content of all candidates in the response parts.
    /// If <typeparamref name="TSchema"/> is <c>string</c>, the raw text is returned; otherwise, the text is deserialized to <typeparamref name="TSchema"/>.
    /// </param>
    /// <param name="Prompt">The original prompt sent to the Gemini API.</param>
    /// <param name="ResponseParts">The collection of all response parts returned by the Gemini API.</param>
    /// <param name="ErrorException">The exception that occurred during the response, if any; otherwise, <c>null</c>.</param>
    internal sealed record GeminiResponse<TSchema>(TSchema? Data, IGeminiPrompt Prompt, IReadOnlyCollection<IGeminiResponseData> ResponseParts, Exception? ErrorException = null) : IGeminiResponse<TSchema>
    {
        /// <summary>
        /// Gets a value indicating whether the response is successful.
        /// Returns <c>true</c> if there is no error exception and the response data is not null; otherwise, <c>false</c>.
        /// </summary>
        public bool IsSuccessful => ErrorException is null && Data is not null;

        /// <summary>
        /// Constructs a <see cref="GeminiResponse{TSchema}"/> from a collection of response parts and the original prompt.
        /// Optionally accepts custom JSON serializer settings for deserialization.
        /// </summary>
        /// <param name="responseParts">The response parts returned by the Gemini API.</param>
        /// <param name="prompt">The original prompt sent to the API.</param>
        /// <param name="serializerSettings">Optional JSON serializer settings for deserialization.</param>
        public GeminiResponse(IReadOnlyCollection<GeminiResponseData> responseParts, IGeminiPrompt prompt, JsonSerializerSettings? serializerSettings = null) : this(GetResponseData(responseParts, serializerSettings), prompt, responseParts) { }

        /// <summary>
        /// Initializes a new <see cref="GeminiResponse{TSchema}"/> instance representing an error response.
        /// Sets the <paramref name="ErrorException"/> property and associates the response with the original prompt.
        /// The <c>Data</c> property will be <c>null</c> and <c>ResponseParts</c> will be empty.
        /// </summary>
        /// <param name="prompt">The original prompt sent to the Gemini API.</param>
        /// <param name="errorException">The exception that occurred during the response.</param>
        public GeminiResponse(IGeminiPrompt prompt, Exception errorException) : this(default, prompt, [], errorException) { }

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
