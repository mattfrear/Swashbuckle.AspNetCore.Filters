using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters.Test
{
    public abstract class BaseOperationFilterTests
    {
        protected OperationFilterContext FilterContextFor(Type controllerType, string actionName)
        {
            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = controllerType.GetTypeInfo(),
                    MethodInfo = controllerType.GetMethod(actionName)
                }
            };

            return new OperationFilterContext(
                apiDescription,
                new SchemaRegistry(new JsonSerializerSettings()),
                (apiDescription.ActionDescriptor as ControllerActionDescriptor).MethodInfo);
        }
    }
}
