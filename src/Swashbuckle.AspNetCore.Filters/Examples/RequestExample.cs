﻿using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class RequestExample
    {
        private readonly JsonFormatter jsonFormatter;
        private readonly SerializerSettingsDuplicator serializerSettingsDuplicator;
        private readonly MvcOutputFormatter mvcOutputFormatter;
        private readonly SwaggerOptions swaggerOptions;

        public RequestExample(
            JsonFormatter jsonFormatter,
            SerializerSettingsDuplicator serializerSettingsDuplicator,
            MvcOutputFormatter mvcOutputFormatter,
            IOptions<SwaggerOptions> options)
        {
            this.jsonFormatter = jsonFormatter;
            this.serializerSettingsDuplicator = serializerSettingsDuplicator;
            this.mvcOutputFormatter = mvcOutputFormatter;
            this.swaggerOptions = options?.Value;
        }

        public void SetRequestExampleForOperation(
            OpenApiOperation operation,
            SchemaRepository schemaRepository,
            Type requestType,
            object example,
            IContractResolver contractResolver = null,
            JsonConverter jsonConverter = null)
        {
            if (example == null)
            {
                return;
            }

            if (operation.RequestBody == null || operation.RequestBody.Content == null)
            {
                return;
            }

            var serializerSettings = serializerSettingsDuplicator.SerializerSettings(contractResolver, jsonConverter);

            var examplesConverter = new ExamplesConverter(jsonFormatter, mvcOutputFormatter, serializerSettings);

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
