using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Any;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    internal abstract class ExampleFormat
    {
        public ExampleFormat(string mime)
        {
            MimeType = MediaTypeHeaderValue.Parse(mime);
        }

        public MediaTypeHeaderValue MimeType { get; }

        public virtual IOpenApiAny Format(string s)
        {
            return new OpenApiRawString(s);
        }
    }
}