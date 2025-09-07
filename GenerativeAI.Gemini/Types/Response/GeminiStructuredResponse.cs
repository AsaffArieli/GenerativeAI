using GenerativeAI.Gemini.Contracts;
using GenerativeAI.Gemini.Models;
using GenerativeAI.Gemini.Models.Part;
using Newtonsoft.Json;

namespace GenerativeAI.Gemini.Types.Response
{
    /// <summary>
    /// Represents a structured response from the Gemini AI model.
    /// </summary>
    /// <typeparam name="TSchema">
    /// The target type for deserialization of the model output. Use <see cref="string"/>
    /// to obtain the raw concatenated text; otherwise, the text is JSON-deserialized
    /// into <typeparamref name="TSchema"/>.
    /// </typeparam>
    public sealed class GeminiStructuredResponse<TSchema> : GeminiResponse
    {
        /// <summary>
        /// Gets the deserialized response data of type <typeparamref name="TSchema"/>.
        /// If <typeparamref name="TSchema"/> is <see cref="string"/>, this is the raw text;
        /// otherwise the text content is deserialized to <typeparamref name="TSchema"/>.
        /// </summary>
        public TSchema? Data { get; }

        /// <summary>
        /// The collection of all response parts returned by the Gemini API.
        /// </summary>
        public IReadOnlyCollection<ResponseData> ResponseParts { get; } = [];

        /// <remarks>
        /// Returns <see langword="true"/> if there is no error exception and the response data is not null; otherwise, <see langword="false"/>.
        /// </remarks>
        /// <inheritdoc cref="GeminiResponse.IsSuccessful" />
        public override bool IsSuccessful => base.IsSuccessful && Data is not null;

        /// <summary>
        /// Constructs a <see cref="GeminiStructuredResponse{TSchema}"/> from a collection of response parts and the original prompt.
        /// Optionally accepts custom JSON serializer settings for deserialization.
        /// </summary>
        /// <param name="responseParts">The response parts returned by the Gemini API.</param>
        /// <param name="prompt">The original prompt sent to the API.</param>
        /// <param name="serializerSettings">Optional JSON serializer settings for deserialization.</param>
        internal GeminiStructuredResponse(IReadOnlyCollection<ResponseData> responseParts, IGeminiPrompt prompt, JsonSerializerSettings? serializerSettings = null) : base(prompt)
        {
            ResponseParts = responseParts;
            Data = GetResponseData(responseParts, serializerSettings);
        }

        /// <inheritdoc cref="GeminiResponse(IGeminiPrompt, Exception)" />
        internal GeminiStructuredResponse(IGeminiPrompt prompt, Exception errorException) : base(prompt, errorException) { }

        /// <summary>
        /// Extracts and deserializes the response data from the provided response parts.
        /// If <typeparamref name="TSchema"/> is <c>string</c>, returns the concatenated text content.
        /// Otherwise, deserializes the text content to <typeparamref name="TSchema"/> using the provided serializer settings.
        /// </summary>
        /// <param name="responseParts">The response parts to extract data from.</param>
        /// <param name="serializerSettings">Optional JSON serializer settings.</param>
        /// <returns>The deserialized response data.</returns>
        private static TSchema? GetResponseData(IReadOnlyCollection<ResponseData> responseParts, JsonSerializerSettings? serializerSettings = null) => string.Join(string.Empty, responseParts.Select(response => response.Candidates.FirstOrDefault()?.Content.Parts.OfType<TextPart>().FirstOrDefault()?.Text)) switch
        {
            string text when typeof(TSchema) == typeof(string) => (TSchema)(object)text,
            string text => JsonConvert.DeserializeObject<TSchema>(text, serializerSettings),
            _ => default
        };
    }
}
