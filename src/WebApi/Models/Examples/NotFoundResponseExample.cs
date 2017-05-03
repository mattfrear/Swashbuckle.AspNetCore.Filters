using Swashbuckle.AspNetCore.Examples;
using WebApi.Controllers;

namespace WebApi.Models.Examples
{
    internal class NotFoundResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ErrorResponse { ErrorCode = 404 };
        }
    }
}