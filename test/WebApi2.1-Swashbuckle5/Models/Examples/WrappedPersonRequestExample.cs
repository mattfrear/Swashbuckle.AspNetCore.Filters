using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class WrappedPersonRequestExample : IExamplesProvider<RequestWrapper<PersonRequest>>
    {
        public RequestWrapper<PersonRequest> GetExamples()
        {
            return new RequestWrapper<PersonRequest>
            {
                Body = new PersonRequest {Title = Title.Ms, Age = 24, FirstName = "Generic Sally", Income = null}
            };
        }
    }
}