using Swashbuckle.AspNetCore.Examples;

namespace WebApi.Models.Examples
{
    internal class WrappedPersonRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new RequestWrapper<PersonRequest>
            {
                Body = new PersonRequest {Title = Title.Ms, Age = 24, FirstName = "Generic Sally", Income = null}
            };
        }
    }
}