using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ExamplesConverter
    {
        private static readonly MediaTypeHeaderValue ApplicationXml = MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");
        private static readonly MediaTypeHeaderValue ApplicationJson = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
        private static readonly MediaTypeHeaderValue TextCsv = MediaTypeHeaderValue.Parse("text/csv");

        private readonly MvcOutputFormatter mvcOutputFormatter;

        public ExamplesConverter(MvcOutputFormatter mvcOutputFormatter)
        {
            this.mvcOutputFormatter = mvcOutputFormatter;
        }

        public JsonNode SerializeExampleCsv(object value)
        {
            var type = value.GetType();
            if (type.IsPrimitive || type.IsValueType || type == typeof(string))
            {
                return value.ToString();
            }

            try
            {
                return mvcOutputFormatter.Serialize(value, TextCsv);
            }
            catch (MvcOutputFormatter.FormatterNotFoundException ex)
            {
                return $"{ex.GetType()}: {ex.Message} for example of {type.FullName}.";
            }
        }

        public JsonNode SerializeExampleXml(object value)
        {
            return mvcOutputFormatter.Serialize(value, ApplicationXml).FormatXml();
        }

        public JsonNode SerializeExampleJson(object value)
        {
            return JsonNode.Parse(mvcOutputFormatter.Serialize(value, ApplicationJson));
        }

        public IDictionary<string, IOpenApiExample> ToOpenApiExamplesDictionaryXml(
            IEnumerable<ISwaggerExample<object>> examples)
        {
            return ToOpenApiExamplesDictionary(examples, SerializeExampleXml);
        }

        public IDictionary<string, IOpenApiExample> ToOpenApiExamplesDictionaryCsv(
            IEnumerable<ISwaggerExample<object>> examples)
        {
            return ToOpenApiExamplesDictionary(examples, SerializeExampleCsv);
        }

        public IDictionary<string, IOpenApiExample> ToOpenApiExamplesDictionaryJson(
            IEnumerable<ISwaggerExample<object>> examples)
        {
            return ToOpenApiExamplesDictionary(examples, SerializeExampleJson);
        }

        private static IDictionary<string, IOpenApiExample> ToOpenApiExamplesDictionary(
            IEnumerable<ISwaggerExample<object>> examples,
            Func<object, JsonNode> exampleConverter)
        {
            var groupedExamples = examples.GroupBy(
                ex => ex.Name,
                ex => (IOpenApiExample)new OpenApiExample
                {
                    Summary = ex.Summary,
                    Description = ex.Description,
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