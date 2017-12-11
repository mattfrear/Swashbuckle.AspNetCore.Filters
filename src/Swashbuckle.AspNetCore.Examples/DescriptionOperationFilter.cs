using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Examples
{
    public class DescriptionOperationFilter : IOperationFilter
    {
        private static readonly SchemaRegistrySettings _settings = new SchemaRegistrySettings();
        private static readonly SchemaIdManager _idManager = new SchemaIdManager(_settings.SchemaIdSelector);
    
        public void Apply(Operation operation, OperationFilterContext context)
        {
            SetResponseModelDescriptions(operation, context.SchemaRegistry, context.ApiDescription);
            SetRequestModelDescriptions(operation, context.SchemaRegistry, context.ApiDescription);
        }

        private static void SetResponseModelDescriptions(Operation operation, ISchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var swaggerResponseAttributes = apiDescription
                .ActionAttributes()
                .Where(r => r.GetType() == typeof(SwaggerResponseAttribute))
                .OfType<SwaggerResponseAttribute>()
                .ToList();

            foreach (var attribute in swaggerResponseAttributes)
            {
                var statusCode = attribute.StatusCode.ToString();

                var response = operation.Responses.FirstOrDefault(r => r.Key == statusCode);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false)
                {
                    if (response.Value != null)
                    {
                        UpdateDescriptions(schemaRegistry, attribute.Type, true);
                    }
                }
            }
        }

        private static void SetRequestModelDescriptions(Operation operation, ISchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            foreach (var parameterDescription in apiDescription.ParameterDescriptions)
            {
                UpdateDescriptions(schemaRegistry, parameterDescription.Type, true);
            }
        }

        private static void UpdateDescriptions(ISchemaRegistry schemaRegistry, Type type, bool recursively = false)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                foreach (var genericArgumentType in type.GetGenericArguments())
                {
                    UpdateDescriptions(schemaRegistry, genericArgumentType, true);
                }
                return;
            }

            if (!schemaRegistry.Definitions.ContainsKey(type.Name))
            {
                return;
            }

            var propertiesWithDescription = type.GetProperties().Where(prop => prop.IsDefined(typeof(DescriptionAttribute), false)).ToList();
            if (!propertiesWithDescription.Any())
            {
                return;
            }

            var definition = schemaRegistry.Definitions[ResolveDefinitionKey(type)];
            foreach (var propertyInfo in propertiesWithDescription)
            {
                UpdatePropertyDescription(propertyInfo, definition);
                if (recursively)
                {
                    UpdateDescriptions(schemaRegistry, propertyInfo.PropertyType, true);
                }
            }
        }

        private static void UpdatePropertyDescription(PropertyInfo prop, Schema schema)
        {
            var propName = ToCamelCase(GetPropertyName(prop));
            if (schema.Properties.ContainsKey(propName))
            {
                var descriptionAttribute = (DescriptionAttribute)prop.GetCustomAttributes(typeof(DescriptionAttribute), false).First();
                schema.Properties[propName].Description = descriptionAttribute.Description;
            }
        }

        private static string GetPropertyName(PropertyInfo prop)
        {
            if (prop.IsDefined(typeof(DataMemberAttribute), false))
            {
                var dataMemberAttribute = (DataMemberAttribute)prop.GetCustomAttributes(typeof(DataMemberAttribute), false).First();
                return dataMemberAttribute.Name ?? prop.Name;
            }
            return prop.Name;
        }

        private static string ResolveDefinitionKey(Type type)
        {
          return _idManager.IdFor(type);
        }
        
        private static string ToCamelCase(string value)
        {
            // lower case the first letter
            return value.Substring(0, 1).ToLower() + value.Substring(1);
        }
    }
}
