namespace GenerativeAI.Gemini
{
    /// <summary>
    /// Defines the contract for a Gemini prompt response.
    /// </summary>
    /// <typeparam name="TSchema">The type to which the response data is deserialized.</typeparam>
    public interface IGeminiResponse<TSchema>
    {
        /// <summary>
        /// Gets the deserialized response data of type <typeparamref name="TSchema"/>.
        /// This is typically constructed from the model's output, either as plain text or as a structured object.
        /// </summary>
        TSchema? Data { get; }

        /// <summary>
        /// Gets a value indicating whether the response is successful.
        /// Returns <c>true</c> if there is no error exception and the response data is not null; otherwise, <c>false</c>.
        /// </summary>
        bool IsSuccessful { get; }

        /// <summary>
        /// Gets the exception that occurred during the request or response processing, if any.
        /// If the response was successful, this value is <c>null</c>. 
        /// If an error occurred, this property contains the relevant <see cref="Exception"/> instance describing the error.
        /// </summary>
        Exception? ErrorException { get; }

        /// <summary>
        /// Get the original prompt that was sent to the Gemini API.
        /// </summary>
        IGeminiPrompt Prompt { get; }

        /// <summary>
        /// Gets the collection of all response parts returned by the Gemini API.
        /// Each part contains candidate responses, usage metadata, model version, and response ID.
        /// </summary>
        IReadOnlyCollection<IGeminiResponseData> ResponseParts { get; }
    }
}
