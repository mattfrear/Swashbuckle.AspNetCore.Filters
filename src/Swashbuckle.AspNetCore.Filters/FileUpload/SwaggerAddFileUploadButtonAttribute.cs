using System;

namespace Swashbuckle.AspNetCore.Filters
{
    [Obsolete("Swashbuckle 4.0 supports IFormFile out of the box")]
    [AttributeUsage(AttributeTargets.Method)]
    public class AddSwaggerFileUploadButtonAttribute : Attribute
    {
    }
}