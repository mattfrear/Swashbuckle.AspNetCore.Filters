using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Examples;

namespace Swashbuckle.AspNetCore.SwaggerGen.Test
{
    public class FakeActions
    {
        [SwaggerResponse(200, typeof(PersonResponse))]
        [SwaggerResponseExample(200, typeof(PersonResponseExample))]
        public IActionResult AnnotatedWithSwaggerResponseExampleAttributes()
        {
            throw new NotImplementedException();
        }

        [SwaggerRequestExample(typeof(PersonRequest), typeof(PersonRequestExample), jsonConverter: typeof(StringEnumConverter))]
        public IActionResult AnnotatedWithSwaggerRequestExampleAttributes(PersonRequest personRequest)
        {
            throw new NotImplementedException();
        }
    }
}