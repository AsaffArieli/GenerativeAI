namespace GenerativeAI.Gemini.Models.Part
{
    /// <summary>
    /// Represents a text content part within a Gemini prompt.
    /// Implements <see cref="IPart"/>.
    /// </summary>
    /// <param name="Text">The text content to include in the prompt.</param>
    public sealed record TextPart(string Text) : IPart;
}
