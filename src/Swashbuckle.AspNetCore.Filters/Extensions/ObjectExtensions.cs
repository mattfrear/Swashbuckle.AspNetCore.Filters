using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Net.Http.Headers;

namespace Swashbuckle.AspNetCore.Filters.Extensions
{
    internal static class ObjectExtensions
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
        private static readonly MediaTypeHeaderValue ApplicationJson = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

        internal static string XmlSerialize<T>(this T value, MvcOutputFormatter mvcOutputFormatter)
        {
            if (mvcOutputFormatter == null)
            {
                throw new ArgumentNullException(nameof(mvcOutputFormatter));
            }

            try
            {
                return mvcOutputFormatter
                    .Serialize(value, ApplicationXml)
                    .FormatXml();
            }
            catch (MvcOutputFormatter.FormatterNotFoundException)
            {
                return value.XmlSerialize();
            }
        }

        internal static string JsonSerialize<T>(this T value, MvcOutputFormatter mvcOutputFormatter)
        {
            if (mvcOutputFormatter == null)
            {
                throw new ArgumentNullException(nameof(mvcOutputFormatter));
            }

            try
            {
                return mvcOutputFormatter
                    .Serialize(value, ApplicationJson);
            }
            catch (MvcOutputFormatter.FormatterNotFoundException)
            {
                return value.XmlSerialize();
            }
        }

        private static string FormatXml(this string unformattedXml)
            => XDocument.Parse(unformattedXml).ToString();
    }
}
