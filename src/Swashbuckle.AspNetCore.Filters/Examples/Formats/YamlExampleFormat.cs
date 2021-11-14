using Microsoft.OpenApi.Any;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    internal class YamlExampleFormat : ExampleFormat
    {
        public YamlExampleFormat() : base("application/yaml") { }

        public override IOpenApiAny Format(string s)
        {
            return new OpenApiString(s);
        }
    }
}