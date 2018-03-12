using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Examples;

namespace Swashbuckle.AspNetCore.SwaggerGen.Test
{
    public class FakeControllers
    {
        public class NotAnnotated
        {}

        public class TestController
        {}

        [Authorize]
        public class AuthController
        { }

        [SwaggerResponse(200, typeof(PersonResponse))]
        [SwaggerResponseExample(200, typeof(PersonResponseExample))]
        public class SwaggerResponseExampleController
        { }
    }
}