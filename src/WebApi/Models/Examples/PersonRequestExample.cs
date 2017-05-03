using Swashbuckle.AspNetCore.Examples;
using WebApi.Controllers;

namespace WebApi.Models.Examples
{
    internal class PersonRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PersonRequest { Age = 24, FirstName = "Dave" };
        }
    }
}