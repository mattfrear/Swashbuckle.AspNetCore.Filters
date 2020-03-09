using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters.Extensions;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ExamplesConverter
    {
        private readonly IJsonFormatter jsonFormatter;
        private readonly MvcOutputFormatter mvcOutputFormatter;

        public ExamplesConverter(IJsonFormatter jsonFormatter, MvcOutputFormatter mvcOutputFormatter)
        {
            this.jsonFormatter = jsonFormatter;
            this.mvcOutputFormatter = mvcOutputFormatter;
        }

        public IOpenApiAny SerializeExampleXml(object exampleValue)
        {
            return new OpenApiString(exampleValue.XmlSerialize(mvcOutputFormatter));
        }

        public IOpenApiAny SerializeExampleJson(object exampleValue)
        {
            return new OpenApiRawString(jsonFormatter.FormatJson(exampleValue));
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
    }
}