namespace Swashbuckle.AspNetCore.Filters.Test.Extensions
{
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Models;

    public static class OpenApiExtensions
    {
        public static T DeserializeDataContractXmlExampleAs<T>(this OpenApiRequestBody response)
        {
            var value = ((OpenApiString)response.Content["application/xml"].Example).Value;
            return DeserializeDataContractXmlAs<T>(value);
        }

        public static T DeserializeDataContractXmlExampleAs<T>(this OpenApiResponse response)
        {
            var value = ((OpenApiString)response.Content["application/xml"].Example).Value;
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