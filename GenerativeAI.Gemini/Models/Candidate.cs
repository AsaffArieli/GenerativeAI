using GenerativeAI.Gemini.Enums;

namespace GenerativeAI.Gemini.Models
{
    /// <summary>
    /// Represents a candidate response from the Gemini AI model.
    /// A candidate contains the generated content and the reason the generation process finished.
    /// </summary>
    /// <param name="Content">The generated content from the model.</param>
    /// <param name="FinishReason">The reason why the model stopped generating this candidate.</param>
    internal sealed record Candidate(Content Content, FinishReason FinishReason);
}
