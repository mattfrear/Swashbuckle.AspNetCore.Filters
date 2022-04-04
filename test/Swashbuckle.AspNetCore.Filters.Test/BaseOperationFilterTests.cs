using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters.Test.Extensions;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Newtonsoft;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;

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
                schemaRepository.GetOrAdd(parameterInfo.ParameterType, parameterInfo.ParameterType.SchemaDefinitionName(), () => new OpenApiSchema()
                {
                    Reference = new OpenApiReference { Id = parameterInfo.Name }
                });
            }

            return FilterContextFor(apiDescription, new CamelCasePropertyNamesContractResolver(), parameterDescriptions, supportedResponseTypes, schemaRepository);
        }

        protected OperationFilterContext FilterContextFor(ApiDescription apiDescription, IContractResolver contractResolver, List<ApiParameterDescription> parameterDescriptions = null, List<ApiResponseType> supportedResponseTypes = null, SchemaRepository schemaRepository = null)
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
                new NewtonsoftSchemaGenerator(schemaOptions, jsonSerializerSettings),
                schemaRepository,
                methodInfo);
        }

        protected void SetSwaggerResponses(OpenApiOperation operation, OperationFilterContext filterContext)
        {
            var swaggerResponseFilter = new AnnotationsOperationFilter();
            swaggerResponseFilter.Apply(operation, filterContext);
        }

        protected static string RenderOpenApiObject(IOpenApiAny item)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, System.Text.Encoding.ASCII, 1024, true))
                {
                    var openApiWriter = new OpenApiJsonWriter(writer);
                    item.Write(openApiWriter, Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0);
                }
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
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
