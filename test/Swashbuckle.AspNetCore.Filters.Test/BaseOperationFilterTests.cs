using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters.Test.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters.Test
{
    public abstract class BaseOperationFilterTests
    {
        protected OperationFilterContext FilterContextFor(Type controllerType, string actionName, List<ApiParameterDescription> parameterDescriptions = null, List<ApiResponseType> supportedResponseTypes = null)
        {
            return FilterContextFor(controllerType, actionName, new CamelCasePropertyNamesContractResolver(), parameterDescriptions, supportedResponseTypes);
        }

        protected OperationFilterContext FilterContextFor(Type controllerType, string actionName, IContractResolver contractResolver, List<ApiParameterDescription> parameterDescriptions = null, List<ApiResponseType> supportedResponseTypes = null)
        {
            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = controllerType.GetTypeInfo(),
                    MethodInfo = controllerType.GetMethod(actionName),
                }
            };

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

            return new OperationFilterContext(
                apiDescription,
                new SchemaGenerator(new SchemaGeneratorOptions(), new JsonSerializerSettings()),
                new SchemaRepository(),
                (apiDescription.ActionDescriptor as ControllerActionDescriptor).MethodInfo);
        }

        protected void SetSwaggerResponses(OpenApiOperation operation, OperationFilterContext filterContext)
        {
            var swaggerResponseFilter = new AnnotationsOperationFilter();
            swaggerResponseFilter.Apply(operation, filterContext);
        }
    }
}
