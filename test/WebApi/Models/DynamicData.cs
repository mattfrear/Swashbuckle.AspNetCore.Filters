using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApi.Models
{
    /// <summary>
    /// This example of dynamic data and using of <see cref="JsonExtensionDataAttribute"/>
    /// </summary>
    /// <remarks>
    /// You can inherit this class to mix static and dynamic property:
    /// <code>
    /// public class MixedData : DynamicData
    /// {
    ///     public string FixedProperty {get;set;}
    /// }
    /// </code>
    /// In this case JSON property "FixedProperty" will be added to FixedProperty, all other to Payload
    /// </remarks>
    public class DynamicData
    {
        [JsonExtensionData]
        public Dictionary<string, object> Payload { get; set; } = new Dictionary<string, object>();
    }
}
