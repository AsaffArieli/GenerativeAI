namespace GenerativeAI.Gemini.Types.PromptConfig
{
    /// <summary>
    /// Represents configuration options for enabling various tools and capabilities in Gemini text prompts.
    /// This class allows you to specify which built-in tools should be available during prompt execution.
    /// </summary>
    public sealed class GeminiTextPromptConfig
    {
        /// <summary>
        /// Indicating whether URL context retrieval is enabled.
        /// When <see langword="true"/>, the model can access and analyze content from web URLs referenced in the prompt.
        /// </summary>
        public bool UrlContext { get; set; }

        /// <summary>
        /// Indicating whether Google Search integration is enabled.
        /// When <see langword="true"/>, the model can perform web searches to gather additional information for responses.
        /// </summary>
        public bool GoogleSearch { get; set; }

        /// <summary>
        /// Indicating whether code execution capabilities are enabled.
        /// When <see langword="true"/>, the model can execute code snippets and return their results as part of the response.
        /// </summary>
        public bool CodeExecution { get; set; }

        /// <summary>
        /// Gets the collection of enabled tools based on the current configuration.
        /// Returns tool objects for each capability that has been enabled.
        /// </summary>
        /// <returns>
        /// An enumerable collection of tool objects corresponding to the enabled capabilities.
        /// Disabled tools (set to <see langword="false"/>) are not included in the result.
        /// </returns>
        internal IEnumerable<object> GetTools() => new object?[]
        {
            UrlContext ? new { UrlContext = new { } } : null,
            GoogleSearch ? new { GoogleSearch = new { } } : null,
            CodeExecution ? new { CodeExecution = new { } } : null
        }.OfType<object>();
    }
}
