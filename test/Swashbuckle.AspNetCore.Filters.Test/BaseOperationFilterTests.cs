using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Filters.Test.Extensions;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Newtonsoft;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Swashbuckle.AspNetCore.Filters.Test
{
    public abstract class BaseOperationFilterTests
    {
        /// <summary>
        /// Used for testing against Minimal APIs / Endpoints
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="parameterDescriptions"></param>
        /// <param name="supportedResponseTypes"></param>
        /// <returns></returns>
        protected OperationFilterContext FilterContextFor(Endpoint endpoint, List<ApiParameterDescription> parameterDescriptions = null, List<ApiResponseType> supportedResponseTypes = null)
        {
            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ActionDescriptor
                {
                    EndpointMetadata = endpoint.Metadata.ToList()
                }
            };

            return FilterContextFor(apiDescription, new CamelCasePropertyNamesContractResolver(), parameterDescriptions, supportedResponseTypes);
        }

        /// <summary>
        /// Used for testing against Controller/Actions
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="actionName"></param>
        /// <param name="parameterDescriptions"></param>
        /// <param name="supportedResponseTypes"></param>
        /// <returns></returns>
        protected OperationFilterContext FilterContextFor(Type controllerType, string actionName, List<ApiParameterDescription> parameterDescriptions = null, List<ApiResponseType> supportedResponseTypes = null)
        {
            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = controllerType.GetTypeInfo(),
                    MethodInfo = controllerType.GetMethod(actionName),
                }
            };

            var schemaRepository = new SchemaRepository();

            var methodInfo = controllerType.GetMethod(actionName);

            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                var dictionary = new Dictionary<string, IOpenApiSchema> { [parameterInfo.Name] = new OpenApiSchema { Type = JsonSchemaType.String } };
                var schema = new OpenApiSchema { Type = JsonSchemaType.Object, Properties =  dictionary };
                var schemaId = parameterInfo.ParameterType.SchemaDefinitionName();

                schemaRepository.AddDefinition(schemaId, schema);
            }

            return FilterContextFor(apiDescription, new CamelCasePropertyNamesContractResolver(), parameterDescriptions, supportedResponseTypes, schemaRepository);
        }

        protected OperationFilterContext FilterContextFor(
            ApiDescription apiDescription,
            IContractResolver contractResolver,
            List<ApiParameterDescription> parameterDescriptions = null,
            List<ApiResponseType> supportedResponseTypes = null,
            SchemaRepository schemaRepository = null)
        {
            if (parameterDescriptions != null)
            {
                apiDescription.With(api => api.ParameterDescriptions, parameterDescriptions);
            }

            if (supportedResponseTypes != null)
            {
                apiDescription.With(api => api.SupportedResponseTypes, supportedResponseTypes);
            }

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            };

            var schemaOptions = new SchemaGeneratorOptions();

            var methodInfo = apiDescription.ActionDescriptor is ControllerActionDescriptor descriptor ?
                descriptor.MethodInfo : null;

            return new OperationFilterContext(
                apiDescription,
                new SchemaGenerator(schemaOptions, new NewtonsoftDataContractResolver(jsonSerializerSettings)),
                schemaRepository,
                new OpenApiDocument(),
                methodInfo);
        }

        protected void SetSwaggerResponses(OpenApiOperation operation, OperationFilterContext filterContext)
        {
            var swaggerResponseFilter = new AnnotationsOperationFilter();
            swaggerResponseFilter.Apply(operation, filterContext);
        }

        static readonly RequestDelegate EmptyRequestDelegate = (context) => Task.CompletedTask;

        protected FakeEndpointConventionBuilder CreateBuilder()
        {
            var conventionBuilder = new FakeEndpointConventionBuilder(new RouteEndpointBuilder(
                EmptyRequestDelegate,
                RoutePatternFactory.Parse("/test"),
                order: 0));

            return conventionBuilder;
        }
    }
}
