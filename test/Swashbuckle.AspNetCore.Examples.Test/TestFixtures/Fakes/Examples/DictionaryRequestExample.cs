using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Examples.Test.TestFixtures.Fakes.Examples
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
