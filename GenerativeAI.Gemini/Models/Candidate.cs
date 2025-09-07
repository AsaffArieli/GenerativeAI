using GenerativeAI.Gemini.Enums;

namespace GenerativeAI.Gemini.Models
{
    /// <summary>
    /// Represents a candidate response from the Gemini AI model.
    /// </summary>
    /// <param name="Content">The generated content from the model.</param>
    /// <param name="FinishReason">The reason why the model stopped generating this candidate.</param>
    public sealed record Candidate(Content Content, FinishReason FinishReason);
}
