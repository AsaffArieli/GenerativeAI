using GenerativeAI.Gemini.Contracts;
using GenerativeAI.Gemini.Models;
using GenerativeAI.Gemini.Models.Part;

namespace GenerativeAI.Gemini.Types.Response
{
    /// <summary>
    /// Represents a text-based response from the Gemini AI model.
    /// </summary>
    public class GeminiTextResponse : GeminiResponse
    {
        /// <summary>
        /// The response payload returned by the Gemini API.
        /// </summary>
        public ResponseData? Data { get; }

        /// <summary>
        /// A collection of all <see cref="TextPart"/> from the Candidates.
        /// </summary>
        public IReadOnlyCollection<TextPart> TextParts => Data?.Candidates.SelectMany(c => c.Content.Parts.OfType<TextPart>()).ToList() ?? [];

        /// <summary>
        /// A collection of all executable code parts from the response.
        /// </summary>
        public IReadOnlyCollection<ExecutableCodePart> ExecutableCodeParts => Data?.Candidates.SelectMany(c => c.Content.Parts.OfType<ExecutableCodePart>()).ToList() ?? [];

        /// <summary>
        /// A collection of all code execution result parts from the response.
        /// </summary>
        public IReadOnlyCollection<ExecutableCodeResultPart> ExecutableCodeResultParts => Data?.Candidates.SelectMany(c => c.Content.Parts.OfType<ExecutableCodeResultPart>()).ToList() ?? [];

        /// <remarks>
        /// Returns <see langword="true"/> if there is no error exception and the response data is not null; otherwise, <see langword="false"/>.
        /// </remarks>
        /// <inheritdoc cref="GeminiResponse.IsSuccessful" />
        public override bool IsSuccessful => base.IsSuccessful && Data is not null;

        internal GeminiTextResponse(ResponseData responseData, IGeminiPrompt prompt) : base(prompt)
        {
            Data = responseData;
        }

        /// <inheritdoc cref="GeminiResponse(IGeminiPrompt, Exception)" />
        internal GeminiTextResponse(IGeminiPrompt prompt, Exception errorException) : base(prompt, errorException) { }
    }
}
