using GenerativeAI.Gemini.Models;

namespace GenerativeAI.Gemini.Types
{
    /// <summary>
    /// Represents the response data payload returned from a Gemini API prompt.
    /// </summary>
    /// <param name="UsageMetadata">Token usage statistics for the prompt and response.</param>
    /// <param name="ModelVersion">The version of the Gemini model that generated the response.</param>
    /// <param name="ResponseId">A unique identifier for this response.</param>
    public sealed record GeminiResponseData(GeminiUsageMetadata UsageMetadata, string ModelVersion, string ResponseId)
    {
        /// <summary>
        /// Gets the collection of candidate responses produced by the model for this request.
        /// </summary>
        internal IEnumerable<Candidate> Candidates { get; } = [];
    };
}
