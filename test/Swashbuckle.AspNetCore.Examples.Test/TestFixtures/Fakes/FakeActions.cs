using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Swashbuckle.AspNetCore.SwaggerGen.Test
{
    public class FakeActions
    {
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public IActionResult AnnotatedWithResponseTypeAttributes()
        {
            throw new NotImplementedException();
        }

        [SwaggerResponse(204, typeof(void), "No content is returned.")]
        [SwaggerResponse(400, typeof(IDictionary<string, string>), "This returns a dictionary.")]
        public IActionResult AnnotatedWithSwaggerResponseAttributes()
        {
            throw new NotImplementedException();
        }
    }
}