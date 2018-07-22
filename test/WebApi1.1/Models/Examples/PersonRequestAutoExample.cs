using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class PersonRequestAutoExample : IExamplesProvider<PersonRequest>
    {
        public PersonRequest GetExamples()
        {
            return new PersonRequest { Title = Title.Mr, Age = 24, FirstName = "Dave Auto!!", Income = null };
        }
    }
}
