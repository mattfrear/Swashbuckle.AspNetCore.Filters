using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Filters.Examples;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ResponseExample
    {
        private readonly MvcOutputFormatter mvcOutputFormatter;

        public ResponseExample(
            MvcOutputFormatter mvcOutputFormatter)
        {
            this.mvcOutputFormatter = mvcOutputFormatter;
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
                SetMultipleResponseExampleForStatusCode(response, multiple, examplesConverter);
            }
        }

        private void SetSingleResponseExampleForStatusCode(
            KeyValuePair<string, OpenApiResponse> response,
            object example,
            ExamplesConverter examplesConverter)
        {
            foreach (var content in response.Value.Content)
            {
                var format = ExampleFormats.GetFormat(content.Key);
                if (format == null)
                    continue; // fail more gracefully?

                content.Value.Example = examplesConverter.SerializeExample(example, format);
            }

        }

        private void SetMultipleResponseExampleForStatusCode(
            KeyValuePair<string, OpenApiResponse> response,
            IEnumerable<ISwaggerExample<object>> examples,
            ExamplesConverter examplesConverter)
        {
            foreach (var content in response.Value.Content)
            {
                var format = ExampleFormats.GetFormat(content.Key);
                if (format == null)
                    continue; // fail more gracefully?

                content.Value.Examples = examplesConverter.ToOpenApiExamplesDictionary(examples, format);
            }
        }
    }
}
