using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class ResponseExample
    {
        private readonly MvcOutputFormatter mvcOutputFormatter;
        private readonly ISchemaGenerator schemaGenerator;

        public ResponseExample(MvcOutputFormatter mvcOutputFormatter,
                ISchemaGenerator schemaGenerator)
        {
            this.mvcOutputFormatter = mvcOutputFormatter;
            this.schemaGenerator = schemaGenerator;
        }

        public void SetResponseExampleForStatusCode(
            OpenApiOperation operation,
            SchemaRepository schemaRepository,
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

            if (example is IEnumerable<ISwaggerExample<object>> multiple)
            {
                var multipleList = multiple.ToList();
                SetMultipleResponseExampleForStatusCode(response, multipleList, examplesConverter);
                SetMultipleSchema(response, schemaRepository, multipleList);
            }
            else
            {
                SetSingleResponseExampleForStatusCode(response, example, examplesConverter);
                SetSingleSchema(response, schemaRepository, example);
            }
        }

        /// <summary>
        /// Sets the operation schema to match exactly the types of the associated examples.
        /// <para>Generates new schemas when needed</para>
        /// </summary>
        private void SetMultipleSchema(KeyValuePair<string, OpenApiResponse> response,
                SchemaRepository schemaRepository,
                List<ISwaggerExample<object>> examples)
        {
            var exampleTypes = examples.Where(x => x.Value != null).Select(x => x.Value?.GetType()).Distinct().ToList();

            if (exampleTypes.Count == 1)
            {
                SetSingleSchema(response, schemaRepository, examples[0].Value);
                return;
            }

            var schemas = exampleTypes.Select(type => schemaGenerator.GenerateSchema(type, schemaRepository)).ToList();

            foreach (var content in response.Value.Content)
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
        private void SetSingleSchema(KeyValuePair<string, OpenApiResponse> response,
                SchemaRepository schemaRepository, object example)
        {
            if (example == null)
                return;

            var exampleType = example.GetType();
            var schemaDefinition = schemaGenerator.GenerateSchema(exampleType, schemaRepository);

            foreach (var content in response.Value.Content)
            {
                if (content.Value.Example == null && !content.Value.Examples.Any())
                    continue;

                content.Value.Schema = schemaDefinition;
            }
        }

        private void SetSingleResponseExampleForStatusCode(
            KeyValuePair<string, OpenApiResponse> response,
            object example,
            ExamplesConverter examplesConverter)
        {
            var jsonExample = new Lazy<IOpenApiAny>(() => examplesConverter.SerializeExampleJson(example));
            var xmlExample = new Lazy<IOpenApiAny>(() => examplesConverter.SerializeExampleXml(example));

            foreach (var content in response.Value.Content)
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
        }

        private void SetMultipleResponseExampleForStatusCode(
            KeyValuePair<string, OpenApiResponse> response,
            IEnumerable<ISwaggerExample<object>> examples,
            ExamplesConverter examplesConverter)
        {
            var jsonExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
                examplesConverter.ToOpenApiExamplesDictionaryJson(examples)
            );

            var xmlExamples = new Lazy<IDictionary<string, OpenApiExample>>(() =>
                examplesConverter.ToOpenApiExamplesDictionaryXml(examples)
            );

            foreach (var content in response.Value.Content)
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
        }
    }
}
