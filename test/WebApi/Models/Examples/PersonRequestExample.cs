using Swashbuckle.AspNetCore.Examples;

namespace WebApi.Models.Examples
{
    internal class PersonRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PersonRequest { Title = Title.Mr, Age = 24, FirstName = "Dave", Income = null };
        }
    }
}