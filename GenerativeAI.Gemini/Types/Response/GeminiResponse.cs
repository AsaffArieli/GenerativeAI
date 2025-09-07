using GenerativeAI.Gemini.Contracts;

namespace GenerativeAI.Gemini.Types.Response
{
    /// <summary>
    /// Represents a response from the Gemini AI model.
    /// </summary>
    public abstract class GeminiResponse(IGeminiPrompt prompt)
    {
        /// <summary>
        /// The prompt associated with this response. It represents the original
        /// prompt supplied to the Gemini API, augmented with the model-generated
        /// candidate content that was appended to its <c>Contents</c> during execution.
        /// </summary>
        public IGeminiPrompt Prompt => prompt;

        /// <summary>
        /// The exception that occurred during the request or response processing, if any.
        /// If the response was successful, this value is <see langword="null"/>. 
        /// If an error occurred, this property contains the relevant <see cref="Exception"/> instance describing the error.
        /// </summary>
        public Exception? ErrorException { get; }

        /// <summary>
        /// Indicating whether the response is successful.
        /// </summary>
        /// <remarks>
        /// Returns <see langword="true"/> if there is no error exception; otherwise, <see langword="false"/>.
        /// </remarks>
        public virtual bool IsSuccessful => ErrorException is null;

        /// <summary>
        /// Initializes a new <see cref="GeminiResponse"/> instance representing an error response.
        /// Sets the <paramref name="errorException"/> property and associates the response with the original prompt.
        /// The <c>Data</c> property will be <c>null</c> and <c>ResponseParts</c> will be empty.
        /// </summary>
        /// <param name="prompt">The original prompt sent to the Gemini API.</param>
        /// <param name="errorException">The exception that occurred during the response.</param>
        internal GeminiResponse(IGeminiPrompt prompt, Exception errorException) : this(prompt)
        {
            ErrorException = errorException;
        }
    }
}
