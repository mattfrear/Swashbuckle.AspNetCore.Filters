﻿using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.Filters.Extensions;

namespace Swashbuckle.AspNetCore.Filters.Examples
{
    internal class XmlExampleFormat : ExampleFormat
    {
        public XmlExampleFormat() : base("application/xml")
        {

        }

        public override IOpenApiAny Format(string s)
        {
            return new OpenApiString(s.FormatXml());
        }
    }
}