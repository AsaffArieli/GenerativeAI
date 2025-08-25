namespace GenerativeAI.Gemini.Models.Part
{
    /// <summary>
    /// Represents a text part within a prompt for the Gemini AI model.
    /// This class encapsulates a single string of text, which can be included as part of a prompt
    /// sent to the Gemini API. It implements <see cref="IPart"/> to allow for consistent handling
    /// of different prompt part types.
    /// </summary>
    /// <param name="Text">The text content to include in the prompt.</param>
    internal sealed record TextPart(string Text) : IPart;
}
