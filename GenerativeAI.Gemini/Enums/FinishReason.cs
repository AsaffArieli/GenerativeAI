using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace GenerativeAI.Gemini.Enums
{
    /// <summary>
    /// Specifies the reason why the Gemini AI model stopped generating a response candidate.
    /// This enum is used to indicate the completion condition for a model's output.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum FinishReason
    {
        /// <summary>
        /// The model stopped because it encountered a natural stop sequence or completed its response.
        /// </summary>
        Stop,

        /// <summary>
        /// The model stopped because the maximum token limit was reached.
        /// </summary>
        [EnumMember(Value = "MAX_TOKENS")]
        MaxTokens,

        /// <summary>
        /// The model stopped due to a safety concern, such as potentially unsafe or sensitive content.
        /// </summary>
        Safety,

        /// <summary>
        /// The model stopped to avoid reciting large amounts of content verbatim (e.g., to prevent plagiarism).
        /// </summary>
        Recitation,

        /// <summary>
        /// The model stopped because the generated content matched a blocklist entry.
        /// </summary>
        Blocklist,

        /// <summary>
        /// The model stopped because the content was identified as prohibited.
        /// </summary>
        [EnumMember(Value = "PROHIBITED_CONTENT")]
        ProhibitedContent,

        /// <summary>
        /// The model stopped for a reason not covered by the other values.
        /// </summary>
        Other,

        /// <summary>
        /// The reason for stopping is unspecified or unknown.
        /// </summary>
        Unspecified
    }
}
