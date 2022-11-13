using System;

namespace Swashbuckle.AspNetCore.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerResponseHeaderAttribute : Attribute
    {
        public SwaggerResponseHeaderAttribute(int statusCode, string name, string type, string description, string format = "")
        {
            StatusCodes = new int[] { statusCode };
            Name = name;
            Type = type;
            Description = description;
            Format = format;
        }

        public SwaggerResponseHeaderAttribute(int[] statusCode, string name, string type, string description, string format = "")
        {
            StatusCodes = statusCode;
            Name = name;
            Type = type;
            Description = description;
            Format = format;
        }

        public int[] StatusCodes { get; }

        public string Name { get; }

        public string Type { get; }

        public string Description { get; }

        public string Format { get; }
    }
}
