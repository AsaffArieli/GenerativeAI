using static GenerativeAI.Gemini.Models.Part.InlineDataPart;

namespace GenerativeAI.Gemini.Models.Part
{
    /// <summary>
    /// Represents an inline data part for attaching a file to a prompt in the Gemini AI model.
    /// This part allows embedding file data directly within the prompt as a base64-encoded string,
    /// along with its associated MIME type.
    /// </summary>
    /// <param name="InlineData">The file data to include in the prompt, containing the base64-encoded content and MIME type.</param>
    internal sealed record InlineDataPart(FileData InlineData) : IPart
    {
        /// <summary>
        /// Encapsulates the data for a file to be included inline in a prompt.
        /// </summary>
        /// <param name="Data">The file content, encoded as a base64 string.</param>
        /// <param name="MimeType">The MIME type of the file.</param>
        public sealed record FileData(string Data, string MimeType);
    }
}
