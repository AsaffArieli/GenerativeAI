using GenerativeAI.Gemini.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace GenerativeAI.Gemini.Serializer
{
    /// <summary>
    /// Custom contract resolver used for JSON serialization in Gemini API calls.
    /// Applies camelCase naming to all properties and supports the <see cref="GeminiPropertyAttribute"/>
    /// to override property names during serialization. This ensures that property names in the serialized
    /// JSON match the expected format and custom names required by the Gemini API.
    /// </summary>
    internal class GeminiContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeminiContractResolver"/> class
        /// with camelCase naming strategy for property names.
        /// </summary>
        public GeminiContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy();
        }

        /// <summary>
        /// Resolves the property name using the base implementation.
        /// </summary>
        /// <param name="propertyName">The original property name.</param>
        /// <returns>The resolved property name.</returns>
        protected override string ResolvePropertyName(string propertyName)
        {
            return base.ResolvePropertyName(propertyName);
        }

        /// <summary>
        /// Creates a <see cref="JsonProperty"/> for the given member, applying the <see cref="GeminiPropertyAttribute"/>
        /// if present to override the property name in the serialized JSON.
        /// </summary>
        /// <param name="member">The member information.</param>
        /// <param name="memberSerialization">The member serialization mode.</param>
        /// <returns>The created <see cref="JsonProperty"/>.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var memberType = member switch
            {
                PropertyInfo prop => prop.PropertyType,
                FieldInfo field => field.FieldType,
                _ => null
            };

            property.PropertyName = member.GetCustomAttribute<GeminiPropertyAttribute>()?.Name ?? memberType?.GetCustomAttribute<GeminiPropertyAttribute>()?.Name ?? property.PropertyName;

            return property;
        }
    }
}