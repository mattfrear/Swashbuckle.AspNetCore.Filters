using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System;

namespace Swashbuckle.AspNetCore.Filters
{
    [Obsolete("Swashbuckle 4.0 supports IFormFile out of the box")]
    public class AddFileParamTypesOperationFilter : IOperationFilter
    {
        private static readonly string[] fileParameters = new[] { "ContentType", "ContentDisposition", "Headers", "Length", "Name", "FileName" };

        public void Apply(Operation operation, OperationFilterContext context)
        {
            var operationHasFileUploadButton = context.MethodInfo.GetCustomAttributes<AddSwaggerFileUploadButtonAttribute>().Any();

            if (!operationHasFileUploadButton)
            {
                return;
            }

            operation.Consumes.Add("multipart/form-data");

            RemoveExistingFileParameters(operation.Parameters);

            operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "file",
                    Required = true,
                    In = "formData",
                    Type = "file",
                    Description = "A file to upload"
                }
            );
        }

        private void RemoveExistingFileParameters(IList<IParameter> operationParameters)
        {
            foreach (var parameter in operationParameters.Where(p => p.In == "query" && fileParameters.Contains(p.Name)).ToList())
            {
                operationParameters.Remove(parameter);
            }
        }
    }
}