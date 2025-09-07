namespace GenerativeAI.Gemini.Models.Part
{
    /// <summary>
    /// Represents executable code content within a Gemini prompt.
    /// Implements <see cref="IPart"/>.
    /// </summary>
    /// <param name="Code">The source code to be executed.</param>
    /// <param name="Language">The programming language of the code.</param>
    public sealed record ExecutableCodePart(string Code, string Language) : IPart;
}
