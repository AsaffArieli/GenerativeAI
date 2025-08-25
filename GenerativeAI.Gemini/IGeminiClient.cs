namespace GenerativeAI.Gemini
{
    /// <summary>
    /// Represents the contract for a Gemini client that manages prompt creation and model execution against the Gemini API.
    /// </summary>
    public interface IGeminiClient
    {
        /// <summary>
        /// Gets or sets the default configuration options used for Gemini prompts and API requests.
        /// </summary>
        GeminiPromptOptions DefaultPromptOptions { get; set; }

        /// <summary>
        /// Creates a new <see cref="IGeminiPrompt"/> instance using the specified options or the default options if none are provided.
        /// </summary>
        /// <param name="defaultOptions">Optional. The prompt options to use for the new prompt. If <c>null</c>, <see cref="DefaultPromptOptions"/> is used.</param>
        /// <returns>A new <see cref="IGeminiPrompt"/> instance.</returns>
        IGeminiPrompt CreatePrompt(GeminiPromptOptions? defaultOptions = null);

        /// <summary>
        /// Executes the Gemini model using the specified prompt and returns a response containing plain text data.
        /// </summary>
        /// <param name="prompt">The prompt to send to the Gemini API.</param>
        /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task representing the asynchronous operation, with a result of <see cref="IGeminiResponse{String}"/> containing the model's response as a string.
        /// </returns>
        Task<IGeminiResponse<string>> ExecuteModelAsync(IGeminiPrompt prompt, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the Gemini model using the specified prompt and returns a response containing data deserialized to the specified schema type.
        /// </summary>
        /// <typeparam name="TSchema">The type to which the response data should be deserialized.</typeparam>
        /// <param name="prompt">The prompt to send to the Gemini API.</param>
        /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task representing the asynchronous operation, with a result of <see cref="IGeminiResponse{TSchema}"/> containing the model's response as the specified type.
        /// </returns>
        Task<IGeminiResponse<TSchema>> ExecuteModelAsync<TSchema>(IGeminiPrompt prompt, CancellationToken cancellationToken = default);
    }
}
