using System;

namespace Swashbuckle.AspNetCore.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerResponseHeaderAttribute : Attribute
    {
        public SwaggerResponseHeaderAttribute(int statusCode, string name, string type, string description)
        {
            StatusCodes = new int[] { statusCode };
            Name = name;
            Type = type;
            Description = description;
        }

        public SwaggerResponseHeaderAttribute(int[] statusCode, string name, string type, string description)
        {
            StatusCodes = statusCode;
            Name = name;
            Type = type;
            Description = description;
        }

        public int[] StatusCodes { get; }

        public string Name { get; }

        public string Type { get; }

        public string Description { get; }
    }
}
