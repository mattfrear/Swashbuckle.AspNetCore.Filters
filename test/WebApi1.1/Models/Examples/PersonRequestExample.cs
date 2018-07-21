using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class PersonRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PersonRequest { Title = Title.Mr, Age = 24, FirstName = "Dave", Income = null };
        }
    }

    internal class PersonRequestAutoExample : IAutoExamplesProvider<PersonRequest>
    {
        public PersonRequest GetExamples()
        {
            return new PersonRequest { Title = Title.Mr, Age = 24, FirstName = "Dave Auto!!", Income = null };
        }
    }
}