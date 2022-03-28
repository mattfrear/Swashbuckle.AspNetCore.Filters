using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class RequestExample
    {
        private readonly MvcOutputFormatter mvcOutputFormatter;
        private readonly ISchemaGenerator schemaGenerator;
        private readonly SwaggerOptions swaggerOptions;

        public RequestExample(
            MvcOutputFormatter mvcOutputFormatter,
            IOptions<SwaggerOptions> options,
            ISchemaGenerator schemaGenerator)
        {
            this.mvcOutputFormatter = mvcOutputFormatter;
            this.schemaGenerator = schemaGenerator;
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
            if (example is IEnumerable<ISwaggerExample<object>> multiple)
            {
                var multipleList = multiple.ToList();
                firstOpenApiExample = SetMultipleRequestExamplesForOperation(operation, multipleList, examplesConverter);
                SetMultipleSchema(operation, schemaRepository, multipleList);
            }
            else
            {
                firstOpenApiExample = SetSingleRequestExampleForOperation(operation, example, examplesConverter);
                SetSingleSchema(operation, schemaRepository, example);
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
        /// Sets the operation schema to match exactly the types of the associated examples.
        /// <para>Generates new schemas when needed</para>
        /// </summary>
        private void SetMultipleSchema(OpenApiOperation operation,
                SchemaRepository schemaRepository,
                List<ISwaggerExample<object>> examples)
        {
            var exampleTypes = examples.Where(x => x.Value != null).Select(x => x.Value?.GetType()).Distinct().ToList();

            if (exampleTypes.Count == 1)
            {
                SetSingleSchema(operation, schemaRepository, examples[0].Value);
                return;
            }

            var schemas = exampleTypes.Select(type => schemaGenerator.GenerateSchema(type, schemaRepository)).ToList();

            foreach (var content in operation.RequestBody.Content)
            {
                if (content.Value.Examples.Count == 0)
                    continue;

                content.Value.Schema = new OpenApiSchema
                {
                    OneOf = schemas
                };
            }
        }

        /// <summary>
        /// Sets the operation schema to match exactly the type of the associated example.
        /// <para>Generates new schema when needed</para>
        /// </summary>
        private void SetSingleSchema(OpenApiOperation operation, SchemaRepository schemaRepository, object example)
        {
            if (example == null)
                return;

            var exampleType = example.GetType();
            var schemaDefinition = schemaGenerator.GenerateSchema(exampleType, schemaRepository);

            foreach (var content in operation.RequestBody.Content)
            {
                if (content.Value.Example == null && !content.Value.Examples.Any())
                    continue;

                content.Value.Schema = schemaDefinition;
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
            var jsonExample = new Lazy<IOpenApiAny>(() => examplesConverter.SerializeExampleJson(example));
            var xmlExample = new Lazy<IOpenApiAny>(() => examplesConverter.SerializeExampleXml(example));

            foreach (var content in operation.RequestBody.Content)
            {
                if (content.Key.Contains("xml"))
                {
                    content.Value.Example = xmlExample.Value;
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
        private IOpenApiAny SetMultipleRequestExamplesForOperation(
            OpenApiOperation operation,
            IEnumerable<ISwaggerExample<object>> examples,
            ExamplesConverter examplesConverter)
        {
            var jsonExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
                examplesConverter.ToOpenApiExamplesDictionaryJson(examples)
            );

            var xmlExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
                examplesConverter.ToOpenApiExamplesDictionaryXml(examples)
            );

            foreach (var content in operation.RequestBody.Content)
            {
                if (content.Key.Contains("xml"))
                {
                    content.Value.Examples = xmlExamples.Value;
                }
                else
                {
                    content.Value.Examples = jsonExamples.Value;
                }
            }

            return operation.RequestBody.Content.FirstOrDefault().Value?.Examples?.FirstOrDefault().Value?.Value;
        }
    }
}
