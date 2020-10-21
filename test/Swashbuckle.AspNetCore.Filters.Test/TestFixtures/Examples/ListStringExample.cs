using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    internal class ListStringExample : IExamplesProvider<IEnumerable<string>>
    {
        public IEnumerable<string> GetExamples()
        {
            return new[] { "Hello", "there" };
        }
    }
}
