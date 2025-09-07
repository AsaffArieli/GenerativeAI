using GenerativeAI.Gemini.Enums;
using System.Runtime.Serialization;

namespace GenerativeAI.Gemini.Types.Prompt
{
    /// <summary>
    /// Represents configuration options for a Gemini prompt.
    /// </summary>
    public record GeminiPromptOptions
    {
        /// <summary>
        /// The <see cref="HttpClient"/> instance used for making API requests. If not set, a default client may be used.
        /// </summary>
        public HttpClient? HttpClient { get; set; }

        /// <summary>
        /// The API key used for authenticating requests to the Gemini API.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// The model name as a string, matching the official Gemini model identifier.
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// Controls the randomness of the model's output. Higher values produce more random results, while lower values make output more deterministic.
        /// </summary>
        public double Temperature { get; set; } = 0.2;

        /// <summary>
        /// The number of highest probability tokens to consider for each generation step. Used for sampling diversity.
        /// </summary>
        public int TopK { get; set; } = 40;

        /// <summary>
        /// The cumulative probability threshold for nucleus sampling. The model considers the smallest set of tokens whose probabilities sum to at least this value.
        /// </summary>
        public double TopP { get; set; } = 0.95;

        /// <summary>
        /// The maximum number of tokens the model can generate in the response. If null, the model uses its default limit.
        /// </summary>
        public int? MaxOutputTokens { get; set; }

        /// <summary>
        /// The budget (in tokens or steps) allocated for the model's internal reasoning before producing a response.
        /// </summary>
        public int ThinkingBudget { get; set; } = -1;

        /// <summary>
        /// Gets the <see cref="GeminiModel"/> enum value corresponding to the specified model name string.
        /// </summary>
        /// <param name="model">The model name string, as defined by the <c>EnumMemberAttribute</c> value (e.g., "gemini-2.5-pro").</param>
        /// <returns>
        /// The matching <see cref="GeminiModel"/> value if found; otherwise, <c>null</c>.
        /// </returns>
        public static GeminiModel? GetModelName(string model) => Enum.GetValues<GeminiModel>()
            .FirstOrDefault(member => (Attribute.GetCustomAttribute(typeof(GeminiModel).GetField(member.ToString())!, typeof(EnumMemberAttribute)) as EnumMemberAttribute)?.Value == model);

        /// <summary>
        /// Gets the model name string corresponding to the specified <see cref="GeminiModel"/> enum value.
        /// </summary>
        /// <param name="model">The <see cref="GeminiModel"/> enum value.</param>
        /// <returns>
        /// The model name string as defined by the <c>EnumMemberAttribute</c> or <c>null</c> if not found.
        /// </returns>
        public static string? GetModelName(GeminiModel model) => typeof(GeminiModel)
            .GetMember(model.ToString())
            .FirstOrDefault()?
            .GetCustomAttributes(false)
            .OfType<EnumMemberAttribute>()
            .FirstOrDefault()?
            .Value;
    }
}
