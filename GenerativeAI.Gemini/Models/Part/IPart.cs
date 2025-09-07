using JsonSubTypes;
using Newtonsoft.Json;

namespace GenerativeAI.Gemini.Models.Part
{
    /// <summary>
    /// Defines the contract for a prompt part in Gemini requests.
    /// </summary>
    /// <seealso cref="TextPart" />
    /// <seealso cref="InlineDataPart" />
    /// <seealso cref="ExecutableCodePart" />
    /// <seealso cref="ExecutableCodeResultPart" />
    [JsonConverter(typeof(JsonSubtypes))]
    [JsonSubtypes.KnownSubTypeWithProperty(typeof(TextPart), nameof(TextPart.Text))]
    [JsonSubtypes.KnownSubTypeWithProperty(typeof(InlineDataPart), nameof(InlineDataPart.InlineData))]
    [JsonSubtypes.KnownSubTypeWithProperty(typeof(ExecutableCodePart), nameof(ExecutableCodePart.Code))]
    [JsonSubtypes.KnownSubTypeWithProperty(typeof(ExecutableCodeResultPart), nameof(ExecutableCodeResultPart.Outcome))]
    public interface IPart { }
}