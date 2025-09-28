using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class RequestExample
    {
        private readonly MvcOutputFormatter mvcOutputFormatter;
        private readonly SwaggerOptions swaggerOptions;

        public RequestExample(
            MvcOutputFormatter mvcOutputFormatter,
            IOptions<SwaggerOptions> options)
        {
            this.mvcOutputFormatter = mvcOutputFormatter;
            this.swaggerOptions = options?.Value;
        }

        public void SetRequestBodyExampleForOperation(
            OpenApiOperation operation,
            SchemaRepository schemaRepository,
            Type requestType,
            object example)
        {
            if (example == null)
            {
                return;
            }

            if (operation.RequestBody == null || operation.RequestBody.Content == null)
            {
                return;
            }

            var examplesConverter = new ExamplesConverter(mvcOutputFormatter);

            JsonNode firstOpenApiExample;
            var multiple = example as IEnumerable<ISwaggerExample<object>>;
            if (multiple == null)
            {
                firstOpenApiExample = SetSingleRequestExampleForOperation(operation, example, examplesConverter);
            }
            else
            {
                // firstOpenApiExample = SetMultipleRequestExamplesForOperation(operation, multiple, examplesConverter);
                firstOpenApiExample = SetSingleRequestExampleForOperation(operation, example, examplesConverter);
            }

            if (swaggerOptions.OpenApiVersion == Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0)
            {
                // Swagger v2 doesn't have a request example on the path
                // Fallback to setting it on the object in the "definitions"

                string schemaDefinitionName = requestType.SchemaDefinitionName();
                if (schemaRepository.Schemas.ContainsKey(schemaDefinitionName))
                {
                    var schemaDefinition = schemaRepository.Schemas[schemaDefinitionName];
                    if (schemaDefinition.Example == null)
                    {
                        schemaDefinition.Examples.Add(firstOpenApiExample);
                    }
                }
            }
        }

        /// <summary>
        /// Sets an example on the operation for all of the operation's content types
        /// </summary>
        /// <returns>The first example so that it can be reused on the definition for V2</returns>
        private JsonNode SetSingleRequestExampleForOperation(
            OpenApiOperation operation,
            object example,
            ExamplesConverter examplesConverter)
        {
            var jsonExample = new Lazy<JsonNode>(() => examplesConverter.SerializeExampleJson(example));
            var xmlExample = new Lazy<string>(() => examplesConverter.SerializeExampleXml(example));
            var csvExample = new Lazy<string>(() => examplesConverter.SerializeExampleCsv(example));

            foreach (var content in operation.RequestBody.Content)
            {
                if (content.Key.Contains("xml"))
                {
                    content.Value.Example = xmlExample.Value;
                }
                else if (content.Key.Contains("csv"))
                {
                    content.Value.Example = csvExample.Value;
                }
                else
                {
                    content.Value.Example = jsonExample.Value;
                }
            }

            return operation.RequestBody.Content.FirstOrDefault().Value?.Example;
        }

        /// <summary>
        /// Sets multiple examples on the operation for all of the operation's content types
        /// </summary>
        /// <returns>The first example so that it can be reused on the definition for V2</returns>
        //private IOpenApiAny SetMultipleRequestExamplesForOperation(
        //    OpenApiOperation operation,
        //    IEnumerable<ISwaggerExample<object>> examples,
        //    ExamplesConverter examplesConverter)
        //{
        //    var jsonExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
        //        examplesConverter.ToOpenApiExamplesDictionaryJson(examples)
        //    );

        //    var xmlExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
        //        examplesConverter.ToOpenApiExamplesDictionaryXml(examples)
        //    );

        //    var csvExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
        //        examplesConverter.ToOpenApiExamplesDictionaryCsv(examples)
        //    );

        //    foreach (var content in operation.RequestBody.Content)
        //    {
        //        if (content.Key.Contains("xml"))
        //        {
        //            content.Value.Examples = xmlExamples.Value;
        //        }
        //        else if (content.Key.Contains("csv"))
        //        {
        //            content.Value.Examples = csvExamples.Value;
        //        }
        //        else
        //        {
        //            content.Value.Examples = jsonExamples.Value;
        //        }
        //    }

        //    return operation.RequestBody.Content.FirstOrDefault().Value?.Examples?.FirstOrDefault().Value?.Value;
        //}
    }
}
