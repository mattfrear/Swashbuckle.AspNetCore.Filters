using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Swashbuckle.AspNetCore.Filters.Extensions
{
    public static class ObjectExtensions
    {
        public static string XmlSerialize<T>(
            this T value,
            OutputFormatterSelector outputFormatterSelector = null)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return outputFormatterSelector == null
                ? UseXmlSerializer(value)
                : UseXmlSerializerFromSelector(value, outputFormatterSelector);
        }

        private static string UseXmlSerializer<T>(T value)
        {
            var xmlSerializer = new XmlSerializer(value.GetType());
            var stringWriter = new StringWriter();
            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlSerializer.Serialize(writer, value);
                var uglyXml = stringWriter.ToString();

                var doc = XDocument.Parse(uglyXml);
                return doc.ToString();
            }
        }

        private static string UseXmlSerializerFromSelector<T>(
            T value,
            OutputFormatterSelector outputFormatterSelector)
        {
            if (outputFormatterSelector == null)
                throw new ArgumentNullException(nameof(outputFormatterSelector));

            using (var stringWriter = new StringWriter())
            {
                var outputFormatterContext = GetOutputFormatterContext(
                    stringWriter,
                    value,
                    value.GetType(),
                    "application/xml; charset=utf-8");

                var xmlFormatter = outputFormatterSelector.SelectFormatter(
                    outputFormatterContext,
                    new List<IOutputFormatter>(),
                    new MediaTypeCollection());

                xmlFormatter.WriteAsync(outputFormatterContext).GetAwaiter().GetResult();
                stringWriter.FlushAsync().GetAwaiter().GetResult();
                return stringWriter.ToString();
            }
        }

        private static OutputFormatterWriteContext GetOutputFormatterContext(
            TextWriter writer,
            object outputValue,
            Type outputType,
            string contentType)
        {
            return new OutputFormatterWriteContext(
                GetHttpContext(contentType),
                (stream, encoding) => writer,
                outputType,
                outputValue);
        }

        private static HttpContext GetHttpContext(string contentType)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers["Accept-Charset"] = MediaTypeHeaderValue.Parse(contentType).Charset.ToString();
            httpContext.Request.ContentType = contentType;

            httpContext.Response.Body = new MemoryStream();
            httpContext.RequestServices =
                new ServiceCollection()
                    .AddSingleton(Options.Create(new MvcOptions()))
                    .BuildServiceProvider();

            return httpContext;
        }
    }
}
