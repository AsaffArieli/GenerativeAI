using System.Runtime.Serialization;

namespace GenerativeAI.Gemini.Enums
{
    /// <summary>
    /// Specifies the format of a property in a structured Gemini prompt, using predefined keywords from the OpenAPI Specification.
    /// </summary>
    public enum GeminiPropertyFormatOption
    {
        /// <summary>
        /// No specific format is specified for the property.
        /// </summary>
        NotSpecified = 0,

        // STRING formats

        /// <summary>
        /// Date and time together, including timezone information. Usually in RFC3339 / ISO 8601 format.
        /// Example: "2023-12-25T10:30:00Z"
        /// </summary>
        [EnumMember(Value = "date-time")]
        DateTime,

        /// <summary>
        /// A full date, as defined by RFC3339, section 5.6.
        /// Example: "2023-12-25"
        /// </summary>
        [EnumMember(Value = "date")]
        DateOnly,

        /// <summary>
        /// A time, as defined by RFC3339, section 5.6.
        /// Example: "10:30:00.123+05:30"
        /// </summary>
        [EnumMember(Value = "time")]
        TimeOnly,

        /// <summary>
        /// An internet email address format.
        /// Example: "user@example.com"
        /// </summary>
        [EnumMember(Value = "email")]
        Email,

        /// <summary>
        /// A Universally Unique Identifier (UUID).
        /// Example: "123e4567-e89b-12d3-a456-426614174000"
        /// </summary>
        [EnumMember(Value = "uuid")]
        Uuid,

        /// <summary>
        /// A Uniform Resource Identifier (URI).
        /// Example: "https://www.example.com/path?query=1"
        /// </summary>
        [EnumMember(Value = "uri")]
        Uri,

        /// <summary>
        /// A base64-encoded string of characters.
        /// Example: "U3dhZ2dlciByb2Nrcw=="
        /// </summary>
        [EnumMember(Value = "byte")]
        Byte,

        /// <summary>
        /// An internet host name.
        /// Example: "www.example.com"
        /// </summary>
        [EnumMember(Value = "hostname")]
        Hostname,

        /// <summary>
        /// An IPv4 address.
        /// Example: "198.51.100.1"
        /// </summary>
        [EnumMember(Value = "ipv4")]
        Ipv4,

        /// <summary>
        /// An IPv6 address.
        /// Example: "2001:0db8:85a3:0000:0000:8a2e:0370:7334"
        /// </summary>
        [EnumMember(Value = "ipv6")]
        Ipv6,

        /// <summary>
        /// A hint for UIs that the value should be obscured.
        /// Example: "s3cr3t-p@ssw0rd"
        /// </summary>
        [EnumMember(Value = "password")]
        Password,

        // NUMBER formats

        /// <summary>
        /// Single-precision floating-point number.
        /// </summary>
        [EnumMember(Value = "float")]
        Float,

        /// <summary>
        /// Double-precision floating-point number (the default for NUMBER).
        /// </summary>
        [EnumMember(Value = "double")]
        Double,

        // INTEGER formats

        /// <summary>
        /// A standard 32-bit signed integer.
        /// </summary>
        [EnumMember(Value = "int32")]
        Int32,

        /// <summary>
        /// A 64-bit signed integer, often called a "long". Useful for large IDs or timestamps.
        /// </summary>
        [EnumMember(Value = "int64")]
        Int64
    }
}
