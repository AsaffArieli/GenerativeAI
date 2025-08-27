using GenerativeAI.Gemini.Types;

namespace GenerativeAI.Gemini.Contracts
{
    /// <summary>
    /// Represents the materialized result of a Gemini model invocation.
    /// Implements <see cref="IGeminiResponse{TSchema}"/>.
    /// </summary>
    /// <typeparam name="TSchema">
    /// The target type for deserialization of the model output. Use <see cref="string"/>
    /// to obtain the raw concatenated text; otherwise, the text is JSON-deserialized
    /// into <typeparamref name="TSchema"/>.
    /// </typeparam>
    /// <remarks>
    /// The <see cref="Prompt"/> returned with this response is the original prompt
    /// augmented with the model-generated candidate content appended to its contents.
    /// </remarks>
    public interface IGeminiResponse<TSchema>
    {
        /// <summary>
        /// Gets the deserialized response data of type <typeparamref name="TSchema"/>.
        /// If <typeparamref name="TSchema"/> is <see cref="string"/>, this is the raw text;
        /// otherwise the text content is deserialized to <typeparamref name="TSchema"/>.
        /// </summary>
        TSchema? Data { get; }

        /// <summary>
        /// Gets the prompt associated with this response. It represents the original
        /// prompt supplied to the Gemini API, augmented with the model-generated
        /// candidate content that was appended to its <c>Contents</c> during execution.
        /// </summary>
        IGeminiPrompt Prompt { get; }

        /// <summary>
        /// Gets the collection of all response parts returned by the Gemini API.
        /// </summary>
        IReadOnlyCollection<GeminiResponseData> ResponseParts { get; }

        /// <summary>
        /// Gets the exception that occurred during the request or response processing, if any.
        /// If the response was successful, this value is <c>null</c>. 
        /// If an error occurred, this property contains the relevant <see cref="Exception"/> instance describing the error.
        /// </summary>
        Exception? ErrorException { get; }

        /// <summary>
        /// Gets a value indicating whether the response is successful.
        /// Returns <c>true</c> if there is no error exception and the response data is not null; otherwise, <c>false</c>.
        /// </summary>
        bool IsSuccessful { get; }
    }
}
