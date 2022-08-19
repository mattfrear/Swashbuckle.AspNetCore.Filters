using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class StringResponseExample : IExamplesProvider<string>
    {
        public string GetExamples()
        {
            return "Hello";
        }
    }
}