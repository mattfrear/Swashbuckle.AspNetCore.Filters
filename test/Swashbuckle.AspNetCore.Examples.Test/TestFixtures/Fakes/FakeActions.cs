using System;
using Microsoft.AspNetCore.Mvc;
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
    }
}