using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using WebApi3._0_Swashbuckle5;
using Xunit;

namespace Swashbuckle.AspNetCore.Filters.Test
{
    public class Swashbuckle5Tests
    {
        [Fact]
        public async Task OpenApi()
        {
            // Arrange
            using var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/swagger/v1/swagger.json");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
