using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;

namespace Swashbuckle.AspNetCore.Filters
{
    /// <summary>
    /// Represents a raw value that should not be encoded
    /// </summary>
    internal class OpenApiRawString : IOpenApiAny, IOpenApiPrimitive
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiRawString"/> class.
        /// </summary>
        /// <param name="value">Raw value</param>
        public OpenApiRawString(string value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public AnyType AnyType => AnyType.Primitive;

        /// <inheritdoc/>
        public PrimitiveType PrimitiveType => PrimitiveType.String;

        /// <summary>
        /// Raw value to be output
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Converts between an <see cref="OpenApiString"/> and a
        /// <see cref="OpenApiRawString"/>
        /// </summary>
        /// <param name="value"><see cref="OpenApiString"/> to convert</param>
        public static implicit operator OpenApiRawString(OpenApiString value)
            => ToOpenApiRawString(value);

        /// <summary>
        /// Converts between an <see cref="OpenApiRawString"/> and a
        /// <see cref="OpenApiString"/>
        /// </summary>
        /// <param name="value"><see cref="OpenApiRawString"/> to convert</param>
        public static implicit operator OpenApiString(OpenApiRawString value)
            => ToOpenApiString(value);

        /// <summary>
        /// Converts between an <see cref="OpenApiString"/> and a
        /// <see cref="OpenApiRawString"/>
        /// </summary>
        /// <param name="value"><see cref="OpenApiString"/> to convert</param>
        /// <returns>New <see cref="OpenApiRawString"/></returns>
        public static OpenApiRawString ToOpenApiRawString(OpenApiString value)
            => new OpenApiRawString(value.Value);

        /// <summary>
        /// Converts between an <see cref="OpenApiRawString"/> and a
        /// <see cref="OpenApiString"/>
        /// </summary>
        /// <param name="value"><see cref="OpenApiRawString"/> to convert</param>
        /// <returns>New <see cref="OpenApiString"/></returns>
        public static OpenApiString ToOpenApiString(OpenApiRawString value)
            => new OpenApiString(value.Value);

        /// <inheritdoc/>
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteRaw(Value);
        }
    }
}
