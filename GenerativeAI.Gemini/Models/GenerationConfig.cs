using GenerativeAI.Gemini.Types.Prompt;
using System.Net.Mime;
using static GenerativeAI.Gemini.Models.GenerationConfig;

namespace GenerativeAI.Gemini.Models
{
    /// <summary>
    /// Represents the configuration for generating a response from the Gemini AI model.
    /// </summary>
    /// <param name="ResponseSchema">
    /// The schema object that defines the expected structure of the model's response.
    /// If set, the response will be returned as JSON; otherwise, as plain text.
    /// </param>
    /// <param name="Temperature">
    /// Controls the randomness of the model's output. Higher values produce more random results, while lower values make output more deterministic.
    /// </param>
    /// <param name="TopK">
    /// The number of highest probability tokens to consider for each generation step. Used for sampling diversity.
    /// </param>
    /// <param name="TopP">
    /// The cumulative probability threshold for nucleus sampling. The model considers the smallest set of tokens whose probabilities sum to at least this value.
    /// </param>
    /// <param name="MaxOutputTokens">
    /// The maximum number of tokens the model can generate in the response. If null, the model uses its default limit.
    /// </param>
    /// <param name="ThinkingConfig">
    /// Settings related to the model's "thinking" process, such as the thinking budget.
    /// </param>
    public sealed record GenerationConfig(object? ResponseSchema, double Temperature, int TopK, double TopP, int? MaxOutputTokens, ThinkingSettings ThinkingConfig)
    {
        /// <summary>
        /// Encapsulates settings related to the model's thinking process.
        /// </summary>
        /// <param name="ThinkingBudget">
        /// The budget (in tokens or steps) allocated for the model's internal reasoning before producing a response.
        /// </param>
        public sealed record ThinkingSettings(int ThinkingBudget);

        /// <summary>
        /// Gets the MIME type of the response based on the presence of a response schema.
        /// Returns "application/json" if a schema is set, otherwise "text/plain".
        /// </summary>
        public string ResponseMimeType => ResponseSchema is not null ? MediaTypeNames.Application.Json : MediaTypeNames.Text.Plain;

        /// <summary>
        /// Initializes a new instance of <see cref="GenerationConfig"/> using <see cref="GeminiPromptOptions"/> and an optional response schema.
        /// </summary>
        /// <param name="promptOptions">The prompt options containing generation parameters.</param>
        /// <param name="responseSchema">The optional response schema for structured output.</param>
        public GenerationConfig(GeminiPromptOptions promptOptions, object? responseSchema = null) : this(
            Temperature: promptOptions.Temperature,
            TopK: promptOptions.TopK,
            TopP: promptOptions.TopP,
            MaxOutputTokens: promptOptions.MaxOutputTokens,
            ResponseSchema: responseSchema,
            ThinkingConfig: new ThinkingSettings(promptOptions.ThinkingBudget)) { }
    }
}