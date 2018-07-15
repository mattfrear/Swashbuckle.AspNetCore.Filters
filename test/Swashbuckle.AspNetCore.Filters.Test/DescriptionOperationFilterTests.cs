using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters.Test
{
    public class DescriptionOperationFilterTests : BaseOperationFilterTests
    {
        private readonly IOperationFilter sut;

        public DescriptionOperationFilterTests()
        {
            sut = new DescriptionOperationFilter();
        }

        public void ShouldSetDescriptionOnResponse()
        {

        }
    }
}
