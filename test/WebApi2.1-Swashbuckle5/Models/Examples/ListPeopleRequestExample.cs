using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Models.Examples
{

    //internal class ListPeopleRequestExample : IExamplesProvider<PeopleRequest>
    //{
    //    public PeopleRequest GetExamples()
    //    {
    //        return new PeopleRequest { Title = Title.Mr, Age = 24, FirstName = "Dave in a list", Income = null };
    //    }
    //}

    internal class ListPeopleRequestExample : IExamplesProvider<IEnumerable<PeopleRequest>>
    {
        public IEnumerable<PeopleRequest> GetExamples()
        {
            return new List<PeopleRequest> { new PeopleRequest { Title = Title.Mr, Age = 24, FirstName = "Dave in a list", Income = null } }.AsEnumerable();
        }
    }
}