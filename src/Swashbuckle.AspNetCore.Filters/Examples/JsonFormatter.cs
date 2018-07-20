using Newtonsoft.Json;
using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class JsonFormatter
    {
        public object FormatJson(object examples, JsonSerializerSettings serializerSettings, bool includeMediaType)
        {
            if (includeMediaType)
            {
                var wrappedExamples = new Dictionary<string, object>
                {
                    {
                        "application/json", examples
                    }
                };

                return SerializeDeserialize(wrappedExamples, serializerSettings);
            }

            return SerializeDeserialize(examples, serializerSettings);
        }

        private static object SerializeDeserialize(object examples, JsonSerializerSettings serializerSettings)
        {
            var jsonString = JsonConvert.SerializeObject(examples, serializerSettings);
            var result = JsonConvert.DeserializeObject(jsonString);
            return result;
        }
    }
}
