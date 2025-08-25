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
        [EnumMember(Value = "gemini-2.5-pro")]
        Gemini2_5Pro,
        [EnumMember(Value = "gemini-2.5-flash")]
        Gemini2_5Flash,
        [EnumMember(Value = "gemini-2.5-flash-lite")]
        Gemini2_5FlashLite,
        [EnumMember(Value = "gemini-2.0-flash")]
        Gemini2Flash,
        [EnumMember(Value = "gemini-2.0-flash-preview-image-generation")]
        Gemini2PreviewImageGenerationLite,
        [EnumMember(Value = "gemini-2.0-flash-lite")]
        Gemini2FlashLite
    }
}
