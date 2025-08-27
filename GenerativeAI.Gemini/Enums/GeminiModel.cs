using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace GenerativeAI.Gemini.Enums
{
    /// <summary>
    /// Specifies the available Gemini model variants that can be used for prompt execution.
    /// Each value corresponds to a specific Gemini model version or capability, as defined by the Gemini API.
    /// The <see cref="EnumMemberAttribute"/> value is used for JSON serialization and API requests.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GeminiModel
    {
        /// <summary>
        /// Gemini 2.5 Pro — general-purpose, higher-quality model suitable for complex tasks.
        /// Serialized as <c>"gemini-2.5-pro"</c>.
        /// </summary>
        [EnumMember(Value = "gemini-2.5-pro")]
        Gemini2_5Pro,

        /// <summary>
        /// Gemini 2.5 Flash — optimized for speed and cost with good quality.
        /// Serialized as <c>"gemini-2.5-flash"</c>.
        /// </summary>
        [EnumMember(Value = "gemini-2.5-flash")]
        Gemini2_5Flash,

        /// <summary>
        /// Gemini 2.5 Flash Lite — lower-latency and cost-focused variant.
        /// Serialized as <c>"gemini-2.5-flash-lite"</c>.
        /// </summary>
        [EnumMember(Value = "gemini-2.5-flash-lite")]
        Gemini2_5FlashLite,

        /// <summary>
        /// Gemini 2.0 Flash — fast, efficient model for general tasks.
        /// Serialized as <c>"gemini-2.0-flash"</c>.
        /// </summary>
        [EnumMember(Value = "gemini-2.0-flash")]
        Gemini2Flash,

        /// <summary>
        /// Gemini 2.0 Flash (preview) with image generation capabilities.
        /// Serialized as <c>"gemini-2.0-flash-preview-image-generation"</c>.
        /// </summary>
        [EnumMember(Value = "gemini-2.0-flash-preview-image-generation")]
        Gemini2PreviewImageGenerationLite,

        /// <summary>
        /// Gemini 2.0 Flash Lite — lightweight, lower-cost variant.
        /// Serialized as <c>"gemini-2.0-flash-lite"</c>.
        /// </summary>
        [EnumMember(Value = "gemini-2.0-flash-lite")]
        Gemini2FlashLite
    }
}
