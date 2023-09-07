using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters.Extensions
{
    internal static class OperationFilterContextExtensions
    {
        public static IEnumerable<T> GetControllerAndActionAttributes<T>(this OperationFilterContext context) where T : Attribute
        {
            var result = new List<T>();

            if (context.MethodInfo != null)
            {
                var controllerAttributes = context.MethodInfo.ReflectedType?.GetTypeInfo().GetCustomAttributes<T>();
                result.AddRange(controllerAttributes);

                var actionAttributes = context.MethodInfo.GetCustomAttributes<T>();
                result.AddRange(actionAttributes);
            }

#if NETCOREAPP3_1_OR_GREATER
            if (context.ApiDescription.ActionDescriptor.EndpointMetadata != null)
            {
                var endpointAttributes = context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<T>();
                result.AddRange(endpointAttributes);
            }
#endif
            return result.Distinct();
        }
    }
}
