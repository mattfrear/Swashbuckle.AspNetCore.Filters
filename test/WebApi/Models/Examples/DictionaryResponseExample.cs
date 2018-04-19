using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Examples;

namespace WebApi.Models.Examples
{
    public class DictionaryResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new Dictionary<string, object>()
            {
                {"PropertyInt", 5},
                {"PropertyString", "Another string"}
            };
        }
    }
}
