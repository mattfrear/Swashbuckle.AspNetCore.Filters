using Newtonsoft.Json;
using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class JsonFormatter
    {
        public string FormatJson(object examples, JsonSerializerSettings serializerSettings, bool includeMediaType)
        {
            serializerSettings.Formatting = Formatting.Indented;

            if (includeMediaType)
            {
                var wrappedExamples = new Dictionary<string, object>
                {
                    {
                        "application/json", examples
                    }
                };

                return JsonConvert.SerializeObject(wrappedExamples, serializerSettings);
            }

            return JsonConvert.SerializeObject(examples, serializerSettings);
        }
    }
}
