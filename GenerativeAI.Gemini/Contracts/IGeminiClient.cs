using GenerativeAI.Gemini.Types.Prompt;
using GenerativeAI.Gemini.Types.PromptConfig;
using GenerativeAI.Gemini.Types.Response;

namespace GenerativeAI.Gemini.Contracts
{
    /// <summary>
    /// Defines the client contract for interacting with Google's Gemini models.
    /// </summary>
    public interface IGeminiClient
    {
        /// <summary>
        /// The default configuration options used for Gemini prompts and API requests.
        /// </summary>
        GeminiPromptOptions DefaultPromptOptions { get; set; }

        /// <summary>
        /// Creates a new <see cref="IGeminiPrompt"/> instance using the specified options or the default options if none are provided.
        /// </summary>
        /// <param name="defaultOptions">Optional. The prompt options to use for the new prompt. If <c>null</c>, <see cref="DefaultPromptOptions"/> is used.</param>
        /// <returns>A new <see cref="IGeminiPrompt"/> instance.</returns>
        IGeminiPrompt CreatePrompt(GeminiPromptOptions? defaultOptions = null);

        /// <summary>
        /// Generates a text response from the Gemini model using the specified prompt and optional tool configuration.
        /// This method is optimized for text-based interactions and provides access to different response part types.
        /// </summary>
        /// <param name="prompt">The prompt to send to the Gemini model.</param>
        /// <param name="config">Optional. Configuration for the prompt.</param>
        /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="GeminiTextResponse"/> containing the model's response.
        /// </returns>
        Task<GeminiTextResponse> GenerateTextAsync(IGeminiPrompt prompt, GeminiTextPromptConfig? config = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a structured response from the Gemini model..
        /// </summary>
        /// <typeparam name="TSchema">
        /// The target type for the model output. 
        /// </typeparam>
        /// <param name="prompt">
        /// The prompt to send to the Gemini model.
        /// </param>
        /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="GeminiStructuredResponse{TSchema}"/> containing the deserialized response data of type <typeparamref name="TSchema"/>.
        /// </returns>
        Task<GeminiStructuredResponse<TSchema>> GenerateObjectAsync<TSchema>(IGeminiPrompt prompt, CancellationToken cancellationToken = default);
    }
}
