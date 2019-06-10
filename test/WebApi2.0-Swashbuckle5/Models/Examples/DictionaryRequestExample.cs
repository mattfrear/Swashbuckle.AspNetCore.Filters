using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace WebApi.Models.Examples
{
    public class DictionaryRequestExample : IExamplesProvider<Dictionary<string, object>>
    {
        public Dictionary<string, object> GetExamples()
        {
            return new Dictionary<string, object>()
            {
                {"PropertyInt", 1},
                {"PropertyString", "Some string"}
            };
        }
    }
}
