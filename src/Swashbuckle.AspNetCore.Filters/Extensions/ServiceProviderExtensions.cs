using System;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters.Extensions
{
    internal static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets a specific IExamplesProvider<T> from the ServiceProvider and calls GetExamples() on it
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="examplesProviderType">Either an IExamplesProvider<T>, or a concrete type, e.g. PersonRequestExample</param>
        /// <returns>The result of calling GetExamples on the given examplesProviderType</returns>
        public static object GetExampleWithExamplesProviderType(this IServiceProvider serviceProvider, Type examplesProviderType)
        {
            var exampleProviderObject = serviceProvider.GetService(examplesProviderType);
            return InvokeGetExamples(examplesProviderType, exampleProviderObject);
        }

        /// <summary>
        /// Searches the serviceProvider for an IExamplesProvider<T>, where T is the requested type
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetExampleForType(this IServiceProvider serviceProvider, Type type)
        {
            if (type == null || type == typeof(void) || IsPrimitiveType())
            {
                return null;
            }

            bool IsPrimitiveType()
            {
                return !type.GetTypeInfo().IsClass
                    && !type.GetTypeInfo().IsGenericType
                    && !type.GetTypeInfo().IsInterface;
            }

            var exampleProviderType = typeof(IExamplesProvider<>).MakeGenericType(type);
            var singleExample = GetExampleWithExamplesProviderType(serviceProvider, exampleProviderType);
            if (singleExample != null)
            {
                return singleExample;
            }

            var multipleExampleProviderType = typeof(IMultipleExamplesProvider<>).MakeGenericType(type);
            return GetExampleWithExamplesProviderType(serviceProvider, multipleExampleProviderType);
        }

        private static object InvokeGetExamples(Type exampleProviderType, object exampleProviderObject)
        {
            if (exampleProviderObject == null)
            {
                return null;
            }

            var methodInfo = exampleProviderType.GetMethod("GetExamples");
            var example = methodInfo.Invoke(exampleProviderObject, null); // yay, we've got the example! Now just need to set it.
            return example;
        }
    }
}
