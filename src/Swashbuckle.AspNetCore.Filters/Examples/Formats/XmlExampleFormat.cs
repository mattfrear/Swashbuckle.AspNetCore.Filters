using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.Filters.Extensions;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    internal class XmlExampleFormat : ExampleFormat
    {
        public XmlExampleFormat() : base("application/xml; charset=utf-8")
        {

        }

        public override IOpenApiAny Format(string s)
        {
            return base.Format(s.FormatXml());
        }
    }
}