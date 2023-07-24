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

#if NETCOREAPP3_1_OR_GREATER
            if (context.ApiDescription.ActionDescriptor.EndpointMetadata != null)
            {
                return context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<T>();
            }
#else
            if (context.MethodInfo != null)
            {
                var controllerAttributes = context.MethodInfo.ReflectedType?.GetTypeInfo().GetCustomAttributes<T>();
                var actionAttributes = context.MethodInfo.GetCustomAttributes<T>();

                var result = new List<T>(actionAttributes);
                if (controllerAttributes != null)
                {
                    result.AddRange(controllerAttributes);
                }

                return result;
            }
#endif
            return Enumerable.Empty<T>();
        }
    }
}
