using Swashbuckle.AspNetCore.Examples;

namespace WebApi.Models.Examples
{
    internal class PersonResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PersonResponse { Id = 123, Title = Title.Dr, FirstName = "John", LastName = "Doe", Age = 27, Income = null };
        }
    }
}