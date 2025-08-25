using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net.Mime;
using System.Runtime.Serialization;

namespace GenerativeAI.Gemini.Enums
{
    /// <summary>
    /// Defines the supported MIME types for files that can be attached to a Gemini prompt.
    /// Each value corresponds to a standard media type string used for content negotiation and file identification.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum GeminiFileMimeTypes
    {
        /// <summary>
        /// Portable Document Format.
        /// MIME type: "application/pdf"
        /// </summary>
        [EnumMember(Value = MediaTypeNames.Application.Pdf)]
        PDF,

        /// <summary>
        /// JavaScript source code.
        /// MIME type: "text/javascript"
        /// </summary>
        [EnumMember(Value = MediaTypeNames.Text.JavaScript)]
        JavaScript,

        /// <summary>
        /// Python source code.
        /// MIME type: "text/x-python"
        /// </summary>
        [EnumMember(Value = "text/x-python")]
        Python,

        /// <summary>
        /// Plain text file.
        /// MIME type: "text/plain"
        /// </summary>
        [EnumMember(Value = MediaTypeNames.Text.Plain)]
        TXT,

        /// <summary>
        /// HTML document.
        /// MIME type: "text/html"
        /// </summary>
        [EnumMember(Value = MediaTypeNames.Text.Html)]
        HTML,

        /// <summary>
        /// Cascading Style Sheets.
        /// MIME type: "text/css"
        /// </summary>
        [EnumMember(Value = MediaTypeNames.Text.Css)]
        CSS,

        /// <summary>
        /// Markdown document.
        /// MIME type: "text/md"
        /// </summary>
        [EnumMember(Value = "text/md")]
        Markdown,

        /// <summary>
        /// Comma-separated values.
        /// MIME type: "text/csv"
        /// </summary>
        [EnumMember(Value = MediaTypeNames.Text.Csv)]
        CSV,

        /// <summary>
        /// XML document.
        /// MIME type: "text/xml"
        /// </summary>
        [EnumMember(Value = MediaTypeNames.Text.Xml)]
        XML,

        /// <summary>
        /// Rich Text Format.
        /// MIME type: "text/rtf"
        /// </summary>
        [EnumMember(Value = MediaTypeNames.Text.Rtf)]
        RTF,

        /// <summary>
        /// PNG image.
        /// MIME type: "image/png"
        /// </summary>
        [EnumMember(Value = MediaTypeNames.Image.Png)]
        PNG,

        /// <summary>
        /// JPEG image.
        /// MIME type: "image/jpeg"
        /// </summary>
        [EnumMember(Value = MediaTypeNames.Image.Jpeg)]
        JPEG,

        /// <summary>
        /// WebP image.
        /// MIME type: "image/webp"
        /// </summary>
        [EnumMember(Value = MediaTypeNames.Image.Webp)]
        WEBP,

        /// <summary>
        /// HEIC image.
        /// MIME type: "image/heic"
        /// </summary>
        [EnumMember(Value = "image/heic")]
        HEIC,

        /// <summary>
        /// HEIF image.
        /// MIME type: "image/heif"
        /// </summary>
        [EnumMember(Value = "image/heif")]
        HEIF,

        /// <summary>
        /// WAV audio.
        /// MIME type: "audio/wav"
        /// </summary>
        [EnumMember(Value = "audio/wav")]
        WAV,

        /// <summary>
        /// MP3 audio.
        /// MIME type: "audio/mp3"
        /// </summary>
        [EnumMember(Value = "audio/mp3")]
        MP3,

        /// <summary>
        /// AIFF audio.
        /// MIME type: "audio/aiff"
        /// </summary>
        [EnumMember(Value = "audio/aiff")]
        AIFF,

        /// <summary>
        /// AAC audio.
        /// MIME type: "audio/aac"
        /// </summary>
        [EnumMember(Value = "audio/aac")]
        AAC,

        /// <summary>
        /// OGG Vorbis audio.
        /// MIME type: "audio/ogg"
        /// </summary>
        [EnumMember(Value = "audio/ogg")]
        OGGVorbis,

        /// <summary>
        /// FLAC audio.
        /// MIME type: "audio/flac"
        /// </summary>
        [EnumMember(Value = "audio/flac")]
        FLAC,
    }
}
