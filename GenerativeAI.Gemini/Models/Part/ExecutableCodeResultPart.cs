namespace GenerativeAI.Gemini.Models.Part
{
    /// <summary>
    /// Represents the result of code execution within a Gemini prompt.
    /// Implements <see cref="IPart"/>.
    /// </summary>
    /// <param name="Outcome">The outcome or status of the code execution.</param>
    /// <param name="Output">The output produced by the executed code.</param>
    public sealed record ExecutableCodeResultPart(string Outcome, string Output) : IPart;
}
