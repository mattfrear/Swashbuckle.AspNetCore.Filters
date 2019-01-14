using System.Net;
using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class WrappedPersonResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ResponseWrapper<PersonResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Body = new PersonResponse
                {
                    Id = 123,
                    Title = Title.Mrs,
                    FirstName = "Generic Jane",
                    LastName = "Doe",
                    Age = 27,
                    Income = null
                }
            };
        }
    }
}