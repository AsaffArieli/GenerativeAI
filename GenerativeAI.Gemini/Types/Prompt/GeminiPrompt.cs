using GenerativeAI.Gemini.Contracts;
using GenerativeAI.Gemini.Enums;
using GenerativeAI.Gemini.Models;
using GenerativeAI.Gemini.Models.Part;
using Newtonsoft.Json;

namespace GenerativeAI.Gemini.Types.Prompt
{
    /// <summary>
    /// Represents a prompt to the Gemini AI model.
    /// </summary>
    /// <param name="options">The configuration options used for this prompt.</param>
    /// <param name="contents">The initial collection of content parts that make up the prompt. If null, an empty collection is used.</param>
    public sealed class GeminiPrompt(GeminiPromptOptions options, ICollection<Content>? contents = null) : IGeminiPrompt
    {
        /// <inheritdoc cref="IGeminiPrompt.Contents" />
        public ICollection<Content> Contents { get; set; } = contents ?? [];

        /// <inheritdoc cref="IGeminiPrompt.PromptOptions" />
        public GeminiPromptOptions PromptOptions { get; set; } = options;

        /// <summary>
        /// Creates a copy of the current prompt instance.
        /// The returned prompt is a deep copy of all data except for the <c>HttpClient</c> property in <see cref="PromptOptions"/>,
        /// which is copied by reference.
        /// </summary>
        /// <returns>A new <see cref="IGeminiPrompt"/> instance with the same contents and options.</returns>
        /// <exception cref="FormatException">Thrown if the contents cannot be deserialized during the cloning process.</exception>
        public IGeminiPrompt Clone()
        {
            var clonedContents = JsonConvert.DeserializeObject<ICollection<Content>>(JsonConvert.SerializeObject(Contents));
            return new GeminiPrompt(PromptOptions with { }, clonedContents ?? throw new FormatException());
        }

        /// <inheritdoc cref="IGeminiPrompt.AddText(string, ContentRole)" />
        public IGeminiPrompt AddText(string text, ContentRole role = ContentRole.User) => AddPart(new TextPart(text), role);

        /// <inheritdoc cref="IGeminiPrompt.AddInlineData(string, string)" />
        public IGeminiPrompt AddInlineData(string base64File, string mimeType) => AddPart(new InlineDataPart(new(base64File, mimeType)));

        /// <summary>
        /// Adds a part to the prompt with the specified role.
        /// If the last content entry has the same role, the part is added to it; otherwise, a new content entry is created.
        /// </summary>
        /// <param name="part">The part to add.</param>
        /// <param name="role">The role associated with the part.</param>
        /// <returns>The current <see cref="GeminiPrompt"/> instance for chaining.</returns>
        private GeminiPrompt AddPart(IPart part, ContentRole role = ContentRole.User)
        {
            if (Contents.LastOrDefault() is { } lastContent && lastContent.Role == role)
                lastContent.Parts.Add(part);
            else
                Contents.Add(new([part], role));
            return this;
        }
    }
}
