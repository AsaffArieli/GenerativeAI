namespace GenerativeAI.Gemini
{
    /// <summary>
    /// Represents token usage statistics for a Gemini prompt and its response.
    /// </summary>
    /// <param name="PromptTokenCount">The number of tokens consumed by the input prompt.</param>
    /// <param name="CandidatesTokenCount">The total number of tokens generated in all candidate responses.</param>
    /// <param name="TotalTokenCount">
    /// The total number of tokens used, including prompt tokens, candidate tokens, and any thinking tokens if applicable.
    /// </param>
    public sealed record GeminiUsageMetadata(long PromptTokenCount, long CandidatesTokenCount, long TotalTokenCount);
}
