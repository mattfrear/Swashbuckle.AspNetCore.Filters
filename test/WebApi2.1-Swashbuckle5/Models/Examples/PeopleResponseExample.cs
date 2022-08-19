using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace WebApi.Models.Examples
{
    internal class PeopleResponseExample : IExamplesProvider<IEnumerable<PersonResponse>>
    {
        public IEnumerable<PersonResponse> GetExamples()
        {
            return new PersonResponse[]
            {
                new PersonResponse
                {
                    Id = 123,
                    Title = Title.Dr,
                    FirstName = "John",
                    LastName = "Doe",
                    Age = 27,
                    Income = null
                },
                new PersonResponse
                {
                    Id = 124,
                    Title = Title.Miss,
                    FirstName = "Angela",
                    LastName = "Doe",
                    Age = 28,
                    Income = null
                }
            };
        }
    }
}