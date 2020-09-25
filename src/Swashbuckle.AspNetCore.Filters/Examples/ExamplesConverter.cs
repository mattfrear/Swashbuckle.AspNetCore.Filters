using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters.Extensions;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ExamplesConverter
    {
        private static readonly MediaTypeHeaderValue ApplicationXml = MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");
        private static readonly MediaTypeHeaderValue ApplicationJson = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

        private readonly JsonFormatter jsonFormatter;
        private readonly MvcOutputFormatter mvcOutputFormatter;
        private readonly JsonSerializerSettings serializerSettings;

        public ExamplesConverter(JsonFormatter jsonFormatter, MvcOutputFormatter mvcOutputFormatter, JsonSerializerSettings serializerSettings)
        {
            this.jsonFormatter = jsonFormatter;
            this.mvcOutputFormatter = mvcOutputFormatter;
            this.serializerSettings = serializerSettings;
        }

        public IOpenApiAny SerializeExampleXml(object value)
        {
            return new OpenApiString(mvcOutputFormatter.Serialize(value, ApplicationXml).FormatXml());
        }

        public IOpenApiAny SerializeExampleJson(object value)
        {
            return new OpenApiRawString(mvcOutputFormatter.Serialize(value, ApplicationJson));
        }

        public IDictionary<string, OpenApiExample> ToOpenApiExamplesDictionaryXml(
            IEnumerable<ISwaggerExample<object>> examples)
        {
            return ToOpenApiExamplesDictionary(examples, SerializeExampleXml);
        }

        public IDictionary<string, OpenApiExample> ToOpenApiExamplesDictionaryJson(
            IEnumerable<ISwaggerExample<object>> examples)
        {
            return ToOpenApiExamplesDictionary(examples, SerializeExampleJson);
        }

        private static IDictionary<string, OpenApiExample> ToOpenApiExamplesDictionary(
            IEnumerable<ISwaggerExample<object>> examples,
            Func<object, IOpenApiAny> exampleConverter)
        {
            var groupedExamples = examples.GroupBy(
                ex => ex.Name,
                ex => new OpenApiExample
                {
                    Summary = ex.Summary,
                    Value = exampleConverter(ex.Value)
                });

            // If names are duplicated, only the first one is taken
            var examplesDict = groupedExamples.ToDictionary(
                grouping => grouping.Key,
                grouping => grouping.First());

            return examplesDict;
        }
    }
}