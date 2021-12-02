using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters.Extensions
{
    internal static class OperationFilterContextExtensions
    {
        public static IEnumerable<T> GetMethodAttributes<T>(this OperationFilterContext context) where T : Attribute
        {
            if (context.MethodInfo != null)
            {
                return context.GetControllerAndActionAttributes<T>();
            }
            #if NETCOREAPP3_1_OR_GREATER
            if (context.ApiDescription.ActionDescriptor.EndpointMetadata != null)
            {
                return context.GetEndpointMetadataAttributes<T>();
            }
            #endif
            return new List<T>();
        }

        private static IEnumerable<T> GetControllerAndActionAttributes<T>(this OperationFilterContext context) where T : Attribute
        {
            var controllerAttributes = context.MethodInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes<T>();
            var actionAttributes = context.MethodInfo.GetCustomAttributes<T>();

            var result = new List<T>(actionAttributes);
            if (controllerAttributes != null)
                result.AddRange(controllerAttributes);
            return result;
        }

        #if NETCOREAPP3_1_OR_GREATER
        private static IEnumerable<T> GetEndpointMetadataAttributes<T>(this OperationFilterContext context) where T : Attribute
        {
            var endpointAttributes = context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<T>();

            var result = new List<T>(endpointAttributes);
            return result;
        }
        #endif
    }
}
