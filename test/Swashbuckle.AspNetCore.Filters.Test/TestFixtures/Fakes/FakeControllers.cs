using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using System;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes
{
    public static class FakeControllers
    {
        public class NotAnnotated
        {}

        public class TestController
        {}

        [Authorize]
        public class AuthController
        {
            [AllowAnonymous]
            public IActionResult AllowAnonymous()
            {
                throw new NotImplementedException();
            }

            [Authorize("Customer")]
            public IActionResult Customer()
            {
                throw new NotImplementedException();
            }

            public IActionResult None()
            {
                throw new NotImplementedException();
            }
        }

        [SwaggerResponse(200, type: typeof(PersonResponse))]
        [SwaggerResponseExample(200, typeof(PersonResponseExample))]
        public class SwaggerResponseExampleController
        {
            public IActionResult None()
            {
                throw new NotImplementedException();
            }
        }
    }
}