using JsonSubTypes;
using Newtonsoft.Json;

namespace GenerativeAI.Gemini.Models.Part
{
    /// <summary>
    /// Defines the interface for all parts that make up a prompt for the Gemini AI model.
    /// Implementations of this interface represent different types of prompt components.
    /// </summary>
    /// <remarks>
    /// This interface is used with polymorphic JSON serialization to support multiple part types.
    /// Known implementations include <see cref="TextPart"/> and <see cref="InlineDataPart"/>.
    /// </remarks>
    [JsonConverter(typeof(JsonSubtypes))]
    [JsonSubtypes.KnownSubTypeWithProperty(typeof(TextPart), nameof(TextPart.Text))]
    [JsonSubtypes.KnownSubTypeWithProperty(typeof(InlineDataPart), nameof(InlineDataPart.InlineData))]
    internal interface IPart { }
}