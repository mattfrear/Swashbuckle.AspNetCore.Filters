using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Net.Http.Headers;

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

            var xmlSerializer = new XmlSerializer(value.GetType());
            var stringWriter = new StringWriter();
            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlSerializer.Serialize(writer, value);

                return stringWriter
                    .ToString()
                    .FormatXml();
            }
        }

        private static readonly MediaTypeHeaderValue ApplicationXml = MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");

        internal static string XmlSerialize<T>(this T value, MvcOutputFormatter mvcOutputFormatter)
        {
            if (mvcOutputFormatter == null)
                throw new ArgumentNullException(nameof(mvcOutputFormatter));

            try
            {
                return mvcOutputFormatter
                    .Serialize(value, ApplicationXml)
                    .FormatXml();
            }
            catch (MvcOutputFormatter.FormatterNotFound)
            {
                return value.XmlSerialize();
            }
        }

        private static string FormatXml(this string unformattedXml)
        {
            var doc = XDocument.Parse(unformattedXml);
            return doc.ToString();
        }
    }
}
