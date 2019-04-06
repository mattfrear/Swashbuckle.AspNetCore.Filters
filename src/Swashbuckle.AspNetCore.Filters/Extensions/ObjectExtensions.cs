using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Swashbuckle.AspNetCore.Filters.Extensions
{
    public static class ObjectExtensions
    {
        public static string XmlSerialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var xmlserializer = new XmlSerializer(typeof(T));
            var stringWriter = new StringWriter();
            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlserializer.Serialize(writer, value);
                return stringWriter.ToString();
            }
        }
    }
}
