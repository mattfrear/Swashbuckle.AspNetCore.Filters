using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    [Obsolete("Use <summary> tags on properties instead")]
    public class DescriptionOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            SetResponseModelDescriptions(operation, context.SchemaRegistry, context);
            SetRequestModelDescriptions(context.SchemaRegistry, context.ApiDescription);
        }

        private static void SetResponseModelDescriptions(Operation operation, ISchemaRegistry schemaRegistry, OperationFilterContext context)
        {
            var actionAttributes = context.MethodInfo.GetCustomAttributes<ProducesResponseTypeAttribute>().ToList();

            foreach (var attribute in actionAttributes)
            {
                var statusCode = attribute.StatusCode.ToString();

                var response = operation.Responses.FirstOrDefault(r => r.Key == statusCode);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false && response.Value != null)
                {
                    UpdateDescriptions(schemaRegistry, attribute.Type);
                }
            }
        }

        private static void SetRequestModelDescriptions(ISchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            foreach (var parameterDescription in apiDescription.ParameterDescriptions)
            {
                if (parameterDescription.Type != null)
                {
                    UpdateDescriptions(schemaRegistry, parameterDescription.Type);
                }
            }
        }

        private static void UpdateDescriptions(ISchemaRegistry schemaRegistry, Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                foreach (var genericArgumentType in type.GetGenericArguments())
                {
                    UpdateDescriptions(schemaRegistry, genericArgumentType);
                }
                return;
            }

            if (type.GetTypeInfo().IsArray)
            {
                UpdateDescriptions(schemaRegistry, type.GetElementType());
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
            }

            var childProperties = type.GetProperties().ToList();
            foreach (var child in childProperties)
            {
                UpdateDescriptions(schemaRegistry, child.PropertyType);
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
            var propName = GetPropertyName(prop);
            foreach (var schemaProperty in schema.Properties)
            {
                if (string.Equals(schemaProperty.Key, propName, StringComparison.OrdinalIgnoreCase))
                {
                    var descriptionAttribute = (DescriptionAttribute)prop.GetCustomAttributes(typeof(DescriptionAttribute), false).First();
                    schemaProperty.Value.Description = descriptionAttribute.Description;
                }
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
    }
}
