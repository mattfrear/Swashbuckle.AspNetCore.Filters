using System.Collections.Generic;
using System;

namespace Swashbuckle.AspNetCore.Filters
{
    public class PolicySelectorWithLabel<T> where T : Attribute
    {
        public Func<IEnumerable<T>, IEnumerable<string>> Selector { get; set; }

        public string Label { get; set; }
    }
}
