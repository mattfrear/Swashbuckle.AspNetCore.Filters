using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters.Extensions;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ResponseExample
    {
        private readonly JsonFormatter jsonFormatter;
        private readonly SerializerSettingsDuplicator serializerSettingsDuplicator;

        public ResponseExample(
            JsonFormatter jsonFormatter,
            SerializerSettingsDuplicator serializerSettingsDuplicator)
        {
            this.jsonFormatter = jsonFormatter;
            this.serializerSettingsDuplicator = serializerSettingsDuplicator;
        }

        public void SetResponseExampleForStatusCode(
            OutputFormatterSelector outputFormatterSelector,
            OpenApiOperation operation,
            int statusCode,
            object example,
            IContractResolver contractResolver = null,
            JsonConverter jsonConverter = null)
        {
            if (example == null)
            {
                return;
            }

            var response = operation.Responses.FirstOrDefault(r => r.Key == statusCode.ToString());

            if (response.Equals(default(KeyValuePair<string, OpenApiResponse>)) == false && response.Value != null)
            {
                var serializerSettings = serializerSettingsDuplicator.SerializerSettings(contractResolver, jsonConverter);
                var jsonExample = new OpenApiRawString(jsonFormatter.FormatJson(example, serializerSettings));

                OpenApiString xmlExample = null;
                if (response.Value.Content.Keys.Any(k => k.Contains("xml")))
                {
                    if (outputFormatterSelector == null)
                    {
                        xmlExample = new OpenApiString(example.XmlSerialize());
                    }
                    else
                    {
                        xmlExample = WriteXml(outputFormatterSelector, example);
                    }
                }

                foreach (var content in response.Value.Content)
                {
                    if (content.Key.Contains("xml"))
                    {
                        content.Value.Example = xmlExample;
                    }
                    else
                    {
                        content.Value.Example = jsonExample;
                    }
                }
            }
        }

        private OpenApiString WriteXml(OutputFormatterSelector outputFormatterSelector, object example)
        {
            using (var stringWriter = new StringWriter())
            {
                var outputFormatterContext = GetOutputFormatterContext(stringWriter, example, example.GetType());

                var xmlFormatter = outputFormatterSelector.SelectFormatter(
                    outputFormatterContext,
                    new List<IOutputFormatter>(),
                    new MediaTypeCollection());

                xmlFormatter.WriteAsync(outputFormatterContext).GetAwaiter().GetResult();
                stringWriter.FlushAsync().GetAwaiter().GetResult();
                return new OpenApiString(stringWriter.ToString());
            }
        }

        private OutputFormatterWriteContext GetOutputFormatterContext(
            TextWriter writer,
            object outputValue,
            Type outputType,
            string contentType = "application/xml; charset=utf-8")
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
