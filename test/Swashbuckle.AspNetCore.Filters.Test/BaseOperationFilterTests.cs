using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters.Test
{
    public abstract class BaseOperationFilterTests
    {
        protected OperationFilterContext FilterContextFor(Type controllerType, string actionName)
        {
            return FilterContextFor(controllerType, actionName, new CamelCasePropertyNamesContractResolver());
        }

        protected OperationFilterContext FilterContextFor(Type controllerType, string actionName, IContractResolver contractResolver)
        {
            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = controllerType.GetTypeInfo(),
                    MethodInfo = controllerType.GetMethod(actionName)
                }
            };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            };

            return new OperationFilterContext(
                apiDescription,
                new SchemaRegistry(jsonSerializerSettings),
                (apiDescription.ActionDescriptor as ControllerActionDescriptor).MethodInfo);
        }


        protected void SetSwaggerResponses(Operation operation, OperationFilterContext filterContext)
        {
            var swaggerResponseFilter = new AnnotationsOperationFilter();
            swaggerResponseFilter.Apply(operation, filterContext);
        }
    }
}
