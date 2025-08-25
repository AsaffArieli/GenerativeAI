namespace GenerativeAI.Gemini.Attributes
{
    /// <summary>
    /// Indicates whether a property should be ignored when generating the schema for a structured Gemini prompt.
    /// Apply this attribute to a property to explicitly include or exclude it from the prompt's schema definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GeminiIgnoreAttribute(bool ignore = true) : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether the property should be ignored in the schema.
        /// </summary>
        public bool Ignore { get; internal set; } = ignore;
    }
}
