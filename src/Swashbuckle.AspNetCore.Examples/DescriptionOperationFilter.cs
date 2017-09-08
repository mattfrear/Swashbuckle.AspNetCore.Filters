using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Examples
{
    public class DescriptionOperationFilter : IOperationFilter
    {
        private static readonly SchemaRegistrySettings Settings = new SchemaRegistrySettings();
        private static readonly SchemaIdManager IdManager = new SchemaIdManager(Settings.SchemaIdSelector);
    
        public void Apply(Operation operation, OperationFilterContext context)
        {
            SetResponseModelDescriptions(operation, context.SchemaRegistry, context.ApiDescription);
        }

        private static void SetResponseModelDescriptions(Operation operation, ISchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var actionAttributes = apiDescription.ActionAttributes();
            var swaggerResponseExampleAttributes = actionAttributes.Where(r => r.GetType() == typeof(SwaggerResponseAttribute));

            foreach (var attribute in swaggerResponseExampleAttributes)
            {
                var attr = (SwaggerResponseAttribute)attribute;
                var statusCode = attr.StatusCode.ToString();

                var response = operation.Responses.FirstOrDefault(r => r.Key == statusCode);

                if (response.Equals(default(KeyValuePair<string, Response>)) == false)
                {
                    if (response.Value != null)
                    {
                        if (schemaRegistry.Definitions.ContainsKey(attr.Type.Name))
                        {
                            var definition = schemaRegistry.Definitions[ResolveDefinitionKey(attr.Type)];

                            var propertiesWithDescription = attr.Type.GetProperties().Where(prop => prop.IsDefined(typeof(DescriptionAttribute), false));

                            foreach (var prop in propertiesWithDescription)
                            {
                                var descriptionAttribute = (DescriptionAttribute)prop.GetCustomAttributes(typeof(DescriptionAttribute), false).First();
                                var propName = ToCamelCase(prop.Name);
                                definition.Properties[propName].Description = descriptionAttribute.Description;
                            }
                        }
                    }
                }
            }
        }
        
        private static string ResolveDefinitionKey(Type type)
        {
          return IdManager.IdFor(type);
        }
        
        private static string ToCamelCase(string value)
        {
            // lower case the first letter
            return value.Substring(0, 1).ToLower() + value.Substring(1);
        }
    }
}
