using Microsoft.AspNetCore.Authorization;

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
    }
}