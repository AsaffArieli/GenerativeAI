namespace GenerativeAI.Gemini.Enums
{
    /// <summary>
    /// Defines whether a property in a structured Gemini prompt should be nullable.
    /// This enum is used to specify if a property can accept null values in the schema definition.
    /// </summary>
    public enum GeminiPropertyNullOption
    {
        /// <summary>
        /// No specific nullability is specified for the property.
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        /// The property is explicitly marked as nullable and can accept null values.
        /// </summary>
        Nullable,

        /// <summary>
        /// The property is explicitly marked as non-nullable and cannot accept null values.
        /// </summary>
        NonNullable
    }
}
