using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters
{
    public interface IMultipleExamplesProvider : IMultipleExamplesProvider<object>
    {
    }

    public interface IMultipleExamplesProvider<T>
    {
        IEnumerable<SwaggerExample<T>> GetExamples();
    }
}
