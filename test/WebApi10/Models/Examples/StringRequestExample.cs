using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class StringRequestExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return "{\"test\":\"string request example\"}";
        }
    }
}