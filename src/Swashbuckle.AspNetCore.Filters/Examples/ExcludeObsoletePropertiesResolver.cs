using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters
{
    /// <summary>
    /// Source: https://stackoverflow.com/a/56694483/32598
    /// </summary>
    public class ExcludeObsoletePropertiesResolver : DefaultContractResolver
    {
        public ExcludeObsoletePropertiesResolver(IContractResolver existingContractResolver)
        {
            // Preserve the naming strategy, which is usually camel case
            var namingStrategy = (existingContractResolver as DefaultContractResolver)?.NamingStrategy;
            if (namingStrategy != null)
            {
                NamingStrategy = namingStrategy;
            }
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);
            if (prop.AttributeProvider.GetAttributes(true).OfType<ObsoleteAttribute>().Any())
            {
                prop.ShouldSerialize = obj => false;
            }

            return prop;
        }
    }
}
