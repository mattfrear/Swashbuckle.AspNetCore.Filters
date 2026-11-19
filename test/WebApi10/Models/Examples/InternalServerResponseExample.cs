using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class InternalServerResponseExample : IExamplesProvider<ErrorResponse>
    {
        public ErrorResponse GetExamples()
        {
            return new ErrorResponse { ErrorCode = 500 };
        }
    }
}