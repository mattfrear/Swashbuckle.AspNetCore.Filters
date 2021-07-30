using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Filters.Examples;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ExamplesConverter
    {
        private readonly MvcOutputFormatter mvcOutputFormatter;

        public ExamplesConverter(MvcOutputFormatter mvcOutputFormatter)
        {
            this.mvcOutputFormatter = mvcOutputFormatter;
        }

        public IOpenApiAny SerializeExample(object value, ExampleFormat format)
        {
            return format.Format(mvcOutputFormatter.Serialize(value, format.MimeType));
        }

        public IDictionary<string, OpenApiExample> ToOpenApiExamplesDictionary(
            IEnumerable<ISwaggerExample<object>> examples,
            ExampleFormat format)
        {
            return ToOpenApiExamplesDictionary(examples, x => SerializeExample(x, format));
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