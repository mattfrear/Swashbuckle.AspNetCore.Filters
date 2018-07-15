using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    public class DescriptionOperationFilter : IOperationFilter
    {
        private static readonly SchemaRegistrySettings _settings = new SchemaRegistrySettings();
        private static readonly SchemaIdManager _idManager = new SchemaIdManager(_settings.SchemaIdSelector);
    
        public void Apply(Operation operation, OperationFilterContext context)
        {
            SetResponseModelDescriptions(operation, context.SchemaRegistry, context);
            SetRequestModelDescriptions(operation, context.SchemaRegistry, context.ApiDescription);
        }

        private static void SetResponseModelDescriptions(Operation operation, ISchemaRegistry schemaRegistry, OperationFilterContext context)
        {
            var swaggerResponseAttributes = context.MethodInfo.GetCustomAttributes<SwaggerResponseAttribute>().ToList();

            foreach (var attribute in swaggerResponseAttributes)
            {
                var statusCode = attribute.StatusCode.ToString();

                var response = operation.Responses.FirstOrDefault(r => r.Key == statusCode);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false && response.Value != null)
                {
                    UpdateDescriptions(schemaRegistry, attribute.Type, true);
                }
            }
        }

        private static void SetRequestModelDescriptions(Operation operation, ISchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            foreach (var parameterDescription in apiDescription.ParameterDescriptions)
            {
                if (parameterDescription.Type != null)
                {
                    UpdateDescriptions(schemaRegistry, parameterDescription.Type, true);
                }
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

            var schema = FindSchemaForType(schemaRegistry, type);
            if (schema == null)
            {
                return;
            }

            var propertiesWithDescription = type.GetProperties().Where(prop => prop.IsDefined(typeof(DescriptionAttribute), false)).ToList();
            if (!propertiesWithDescription.Any())
            {
                return;
            }

            foreach (var propertyInfo in propertiesWithDescription)
            {
                UpdatePropertyDescription(propertyInfo, schema);
                if (recursively)
                {
                    UpdateDescriptions(schemaRegistry, propertyInfo.PropertyType, true);
                }
            }
        }

        private static Schema FindSchemaForType(ISchemaRegistry schemaRegistry, Type type)
        {
            if (schemaRegistry.Definitions.ContainsKey(type.FriendlyId(false)))
            {
                return schemaRegistry.Definitions[type.FriendlyId(false)];
            }

            if (schemaRegistry.Definitions.ContainsKey(type.FriendlyId(true)))
            {
                return schemaRegistry.Definitions[type.FriendlyId(true)];
            }

            return null;
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
            else if (prop.IsDefined(typeof(JsonPropertyAttribute), false))
            {
                var jsonPropertyAttribute = (JsonPropertyAttribute)prop.GetCustomAttributes(typeof(JsonPropertyAttribute), false).First();
                return jsonPropertyAttribute.PropertyName ?? prop.Name;
            }

            return prop.Name;
        }
        
        private static string ToCamelCase(string value)
        {
            // lower case the first letter
            return value.Substring(0, 1).ToLower() + value.Substring(1);
        }
    }
}
