using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Filters.Examples;

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

        public void SetRequestExampleForOperation(
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

            IOpenApiAny firstOpenApiExample;
            var multiple = example as IEnumerable<ISwaggerExample<object>>;
            if (multiple == null)
            {
                firstOpenApiExample = SetSingleRequestExampleForOperation(operation, example, examplesConverter);
            }
            else
            {
                firstOpenApiExample = SetMultipleRequestExamplesForOperation(operation, multiple, examplesConverter);
            }

            if (swaggerOptions.SerializeAsV2)
            {
                // Swagger v2 doesn't have a request example on the path
                // Fallback to setting it on the object in the "definitions"

                string schemaDefinitionName = requestType.SchemaDefinitionName();
                if (schemaRepository.Schemas.ContainsKey(schemaDefinitionName))
                {
                    var schemaDefinition = schemaRepository.Schemas[schemaDefinitionName];
                    if (schemaDefinition.Example == null)
                    {
                        schemaDefinition.Example = firstOpenApiExample;
                    }
                }
            }
        }

        /// <summary>
        /// Sets an example on the operation for all of the operation's content types
        /// </summary>
        /// <returns>The first example so that it can be reused on the definition for V2</returns>
        private IOpenApiAny SetSingleRequestExampleForOperation(
            OpenApiOperation operation,
            object example,
            ExamplesConverter examplesConverter)
        {
            foreach (var content in operation.RequestBody.Content)
            {
                var format = ExampleFormats.GetFormat(content.Key);
                if (format == null)
                    continue; // fail more gracefully?

                content.Value.Example = examplesConverter.SerializeExample(example, format);
            }

            return operation.RequestBody.Content.FirstOrDefault().Value?.Example;
        }

        /// <summary>
        /// Sets multiple examples on the operation for all of the operation's content types
        /// </summary>
        /// <returns>The first example so that it can be reused on the definition for V2</returns>
        private IOpenApiAny SetMultipleRequestExamplesForOperation(
            OpenApiOperation operation,
            IEnumerable<ISwaggerExample<object>> examples,
            ExamplesConverter examplesConverter)
        {
            foreach (var content in operation.RequestBody.Content)
            {
                var format = ExampleFormats.GetFormat(content.Key);
                if (format == null)
                    continue; // fail more gracefully?

                content.Value.Examples = examplesConverter.ToOpenApiExamplesDictionary(examples, format);
            }

            return operation.RequestBody.Content.FirstOrDefault().Value?.Examples?.FirstOrDefault().Value?.Value;
        }
    }
}
