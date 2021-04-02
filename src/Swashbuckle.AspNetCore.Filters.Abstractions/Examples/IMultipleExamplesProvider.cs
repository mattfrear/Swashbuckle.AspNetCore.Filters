using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters
{
    public interface IMultipleExamplesProvider<T>
    {
        IEnumerable<SwaggerExample<T>> GetExamples();
    }
}
