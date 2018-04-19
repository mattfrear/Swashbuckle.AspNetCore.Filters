using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Examples;

namespace WebApi.Models.Examples
{
    public class DictionaryRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new Dictionary<string, object>()
            {
                {"PropertyInt", 1},
                {"PropertyString", "Some string"}
            };
        }
    }
}
