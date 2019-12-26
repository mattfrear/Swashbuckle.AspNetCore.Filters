using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace WebApi.Models.Examples
{
    internal class ListPeopleRequestExample : IExamplesProvider<PeopleRequest>
    {
        public PeopleRequest GetExamples()
        {
            return new PeopleRequest { Title = Title.Mr, Age = 24, FirstName = "Dave in a list", Income = null };
        }
    }
}