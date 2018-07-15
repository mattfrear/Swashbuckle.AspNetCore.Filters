using System;

namespace Swashbuckle.AspNetCore.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerResponseHeaderAttribute : Attribute
    {
        public SwaggerResponseHeaderAttribute(int statusCode, string name, string type, string description)
        {
            StatusCode = statusCode;
            Name = name;
            Type = type;
            Description = description;
        }

        public int StatusCode { get; }

        public string Name { get; }

        public string Type { get; }

        public string Description { get; }
    }
}
