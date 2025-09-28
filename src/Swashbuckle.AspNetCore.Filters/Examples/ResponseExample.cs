using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ResponseExample
    {
        private readonly MvcOutputFormatter mvcOutputFormatter;
        private readonly SwaggerOptions swaggerOptions;

        public ResponseExample(
            MvcOutputFormatter mvcOutputFormatter,
            IOptions<SwaggerOptions> options)
        {
            this.mvcOutputFormatter = mvcOutputFormatter;
            this.swaggerOptions = options?.Value;
        }

        public void SetResponseExampleForStatusCode(
            OpenApiOperation operation,
            int statusCode,
            object example)
        {
            if (example == null)
            {
                return;
            }

            var key = statusCode == 0 ? "default" : statusCode.ToString();
            var response = operation.Responses.FirstOrDefault(r => r.Key == key);

            if (response.Equals(default(KeyValuePair<string, OpenApiResponse>)) || response.Value == null)
            {
                return;
            }

            var examplesConverter = new ExamplesConverter(mvcOutputFormatter);

            var multiple = example as IEnumerable<ISwaggerExample<object>>;
            if (multiple == null)
            {
                SetSingleResponseExampleForStatusCode(response, example, examplesConverter);
            }
            else
            {
                if (swaggerOptions.OpenApiVersion == Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0)
                {
                    // Swashbuckle.AspNetCore 8 & 9 duplicates the examples when using 2.0, which it shouldn't do.
                    // As a workaround I'm going to just set the first example
                    SetSingleResponseExampleForStatusCode(response, multiple.First().Value, examplesConverter);
                }
                else
                {
                    // SetMultipleResponseExampleForStatusCode(response, multiple, examplesConverter);
                }
            }
        }

        private void SetSingleResponseExampleForStatusCode(
            KeyValuePair<string, IOpenApiResponse> response,
            object example,
            ExamplesConverter examplesConverter)
        {
            var jsonExample = new Lazy<JsonNode>(() => examplesConverter.SerializeExampleJson(example));
            var xmlExample = new Lazy<string>(() => examplesConverter.SerializeExampleXml(example));
            var csvExample = new Lazy<string>(() => examplesConverter.SerializeExampleCsv(example));

            foreach (var content in response.Value.Content)
            {
                if (content.Key.Contains("csv"))
                {
                    content.Value.Example = csvExample.Value;
                }
                else if (content.Key.Contains("xml"))
                {
                    content.Value.Example = xmlExample.Value;
                }
                else
                {
                    content.Value.Example = jsonExample.Value;
                }
            }
        }

    //    private void SetMultipleResponseExampleForStatusCode(
    //        KeyValuePair<string, OpenApiResponse> response,
    //        IEnumerable<ISwaggerExample<object>> examples,
    //        ExamplesConverter examplesConverter)
    //    {
    //        var jsonExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
    //            examplesConverter.ToOpenApiExamplesDictionaryJson(examples)
    //        );

    //        var xmlExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
    //            examplesConverter.ToOpenApiExamplesDictionaryXml(examples)
    //        );

    //        var csvExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
    //            examplesConverter.ToOpenApiExamplesDictionaryCsv(examples)
    //        );

    //        foreach (var content in response.Value.Content)
    //        {
    //            if (content.Key.Contains("xml"))
    //            {
    //                content.Value.Examples = xmlExamples.Value;
    //            }
    //            else if (content.Key.Contains("csv"))
    //            {
    //                content.Value.Examples = csvExamples.Value;
    //            }
    //            else
    //            {
    //                content.Value.Examples = jsonExamples.Value;
    //            }
    //        }
    //    }
    }
}
