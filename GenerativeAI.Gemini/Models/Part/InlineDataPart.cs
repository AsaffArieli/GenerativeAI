using static GenerativeAI.Gemini.Models.Part.InlineDataPart;

namespace GenerativeAI.Gemini.Models.Part
{
    /// <summary>
    /// Represents an inline file content part within a Gemini prompt.
    /// Implements <see cref="IPart"/>.
    /// </summary>
    /// <param name="InlineData">The file data to include in the prompt, containing the base64-encoded content and MIME type.</param>
    public sealed record InlineDataPart(FileData InlineData) : IPart
    {
        /// <summary>
        /// Represents a file payload for inline inclusion in a prompt.
        /// </summary>
        /// <param name="Data">The file content, encoded as a base64 string.</param>
        /// <param name="MimeType">The MIME type of the file.</param>
        public sealed record FileData(string Data, string MimeType);
    }
}
