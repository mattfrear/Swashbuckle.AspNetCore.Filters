using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Microsoft.OpenApi;

namespace Swashbuckle.AspNetCore.Filters.Test.Extensions
{
    public static class OpenApiExtensions
    {
        public static T DeserializeDataContractXmlExampleAs<T>(this OpenApiRequestBody response)
        {
            var value = response.Content["application/xml"].Example.ToString();
            return DeserializeDataContractXmlAs<T>(value);
        }

        public static T DeserializeDataContractXmlExampleAs<T>(this OpenApiResponse response)
        {
            var value = response.Content["application/xml"].Example.ToString();
            return DeserializeDataContractXmlAs<T>(value);
        }

        private static T DeserializeDataContractXmlAs<T>(string value)
        {
            var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(value), new XmlDictionaryReaderQuotas());
            var deserializer = new DataContractSerializer(typeof(T));
            return (T)deserializer.ReadObject(reader);
        }
    }
}