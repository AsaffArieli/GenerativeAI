using GenerativeAI.Gemini.Enums;
using GenerativeAI.Gemini.Models.Part;

namespace GenerativeAI.Gemini.Models
{
    /// <summary>
    /// Represents a content part in the Gemini API, defining both the role (origin) and the content parts themselves.
    /// </summary>
    /// <param name="Parts">The collection of content parts.</param>
    /// <param name="Role">The role indicating whether the content is from the user or the model.</param>
    public sealed record Content(ICollection<IPart> Parts, ContentRole Role);
}
