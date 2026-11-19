using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    public class DynamicDataResponseExample : IExamplesProvider<DynamicData>
    {
        public DynamicData GetExamples()
        {
            var ret = new DynamicData();
            ret.Payload.Add("DynamicProp", 12);
            ret.Payload.Add("Another", "String data");
            return ret;
        }
    }
}
