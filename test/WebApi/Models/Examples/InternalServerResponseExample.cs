using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class InternalServerResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ErrorResponse { ErrorCode = 500 };
        }
    }
}