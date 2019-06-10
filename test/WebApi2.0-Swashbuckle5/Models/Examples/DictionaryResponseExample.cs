using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace WebApi.Models.Examples
{
    public class DictionaryResponseExample : IExamplesProvider<Dictionary<string, object>>
    {
        public Dictionary<string, object> GetExamples()
        {
            return new Dictionary<string, object>()
            {
                {"PropertyInt", 5},
                {"PropertyString", "Another string"}
            };
        }
    }
}
