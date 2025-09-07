using GenerativeAI.Gemini.Types;

namespace GenerativeAI.Gemini.Models
{
    /// <summary>
    /// Represents the response data payload returned from a Gemini API prompt.
    /// </summary>
    /// <param name="UsageMetadata">Token usage statistics for the prompt and response.</param>
    /// <param name="ModelVersion">The version of the Gemini model that generated the response.</param>
    /// <param name="ResponseId">A unique identifier for this response.</param>
    /// <param name="Candidates">The collection of candidate responses produced by the model for this request.</param>
    public sealed record ResponseData(
        GeminiUsageMetadata UsageMetadata,
        string ModelVersion,
        string ResponseId,
        IEnumerable<Candidate> Candidates
    );
}
