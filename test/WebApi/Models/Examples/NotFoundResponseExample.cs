using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class NotFoundResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ErrorResponse { ErrorCode = 404, Message = "The entity was not found" };
        }
    }
}