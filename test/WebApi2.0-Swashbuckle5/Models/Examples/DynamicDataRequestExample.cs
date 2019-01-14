using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    public class DynamicDataRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            var ret = new DynamicData();
            ret.Payload.Add("DynamicProp", 1);
            return ret;
        }
    }
}
