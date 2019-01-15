using Microsoft.OpenApi.Any;

namespace Swashbuckle.AspNetCore.Filters.Extensions
{
    public static class ObjectExtensions
    {
        public static OpenApiObject ToOpenApiObject(this object value)
        {
            var result = new OpenApiObject();
            foreach (var property in value.GetType().GetProperties())
            {
                result.Add(property.Name, new OpenApiString(property.GetValue(value).ToString()));
            }

            return result;
        }
    }
}
