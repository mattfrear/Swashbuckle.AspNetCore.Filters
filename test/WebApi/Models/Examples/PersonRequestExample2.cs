using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class PersonRequestExample2 : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PersonRequest { Title = Title.Miss, Age = 32, FirstName = "Angela", Income = null };
        }
    }
}