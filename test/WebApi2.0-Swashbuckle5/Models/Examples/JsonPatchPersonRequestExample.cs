using Microsoft.AspNetCore.JsonPatch.Operations;
using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    public class JsonPatchPersonRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new[]
            {
                new Operation
                {
                    op = "replace",
                    path = "/firstname",
                    value = "Steve"
                },
                new Operation
                {
                    op = "remove",
                    path = "/income"
                }
            };
        }
    }
}
