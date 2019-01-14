using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class PersonResponseExample2 : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PersonResponse { Id = 123, Title = Title.Dr, FirstName = "Hank", LastName = "Thomas", Age = 27, Income = null };
        }
    }
}