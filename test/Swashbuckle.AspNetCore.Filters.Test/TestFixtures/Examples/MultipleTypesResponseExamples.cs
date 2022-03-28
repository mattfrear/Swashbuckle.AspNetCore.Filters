using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    public class MultipleTypesResponseExamples : IMultipleExamplesProvider
    {
        public IEnumerable<SwaggerExample<object>> GetExamples()
        {
            yield return new SwaggerExample
            {
                Name = "Animal",
                Summary = "Lovely dog",
                Value = new AnimalResponse
                {
                    Id = 29,
                    Name = "Milo",
                    Race = "Husky",
                    IsNeutered = true
                }
            };

            yield return new SwaggerExample
            {
                Name = "Human",
                Summary = "Humble person",
                Value = new PersonResponse
                {
                    Id = 78,
                    FirstName = "Anthony",
                    LastName = "Brown",
                    Age = 25,
                    Title = Title.Dr,
                    Income = 4220.58M,
                }
            };
        }
    }
}
