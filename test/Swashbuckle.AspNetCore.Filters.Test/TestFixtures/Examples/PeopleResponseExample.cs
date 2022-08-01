using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    internal class PeopleResponseExample : IExamplesProvider<IEnumerable<PersonResponse>>
    {
        public IEnumerable<PersonResponse> GetExamples()
        {
            return new[]
            {
                new PersonResponse { Id = 123, Title = Title.Dr, FirstName = "John", LastName = "Doe", Age = 27, Income = null },
                new PersonResponse { Id = 456, Title = Title.Dr, FirstName = "Jane", LastName = "Smith", Age = 26, Income = null }
            };
        }
    }
}