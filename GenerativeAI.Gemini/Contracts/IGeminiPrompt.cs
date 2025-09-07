using GenerativeAI.Gemini.Enums;
using GenerativeAI.Gemini.Models;
using GenerativeAI.Gemini.Types.Prompt;

namespace GenerativeAI.Gemini.Contracts
{
    /// <summary>
    /// Defines the contract for building a prompt to be sent to the Gemini AI model.
    /// </summary>
    public interface IGeminiPrompt
    {
        /// <summary>
        /// The collection of content parts that make up the prompt.
        /// </summary>
        ICollection<Content> Contents { get; set; }

        /// <summary>
        /// The configuration options used for this prompt.
        /// </summary>
        GeminiPromptOptions PromptOptions { get; }

        /// <summary>
        /// Creates a copy of the current prompt instance.
        /// The returned prompt is a deep copy of all data except for the <c>HttpClient</c> property in <see cref="PromptOptions"/>,
        /// which is copied by reference.
        /// </summary>
        /// <returns>A new <see cref="IGeminiPrompt"/> instance with the same contents and options.</returns>
        IGeminiPrompt Clone();

        /// <summary>
        /// Adds a text part to the prompt with the specified role (default is user).
        /// </summary>
        /// <param name="text">The text to add to the prompt.</param>
        /// <param name="role">The role associated with the text (user or model).</param>
        /// <returns>The current <see cref="IGeminiPrompt"/> instance for chaining.</returns>
        IGeminiPrompt AddText(string text, ContentRole role = ContentRole.User);

        /// <summary>
        /// Adds an inline file (as base64-encoded data) to the prompt.
        /// </summary>
        /// <param name="base64File">The file content encoded as a base64 string.</param>
        /// <param name="mimeType">The MIME type of the file.</param>
        /// <returns>The current <see cref="IGeminiPrompt"/> instance for chaining.</returns>
        IGeminiPrompt AddInlineData(string base64File, string mimeType);
    }
}
