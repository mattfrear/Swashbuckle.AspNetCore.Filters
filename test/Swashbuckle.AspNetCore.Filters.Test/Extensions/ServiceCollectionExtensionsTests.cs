using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using Xunit;

namespace Swashbuckle.AspNetCore.Filters.Test.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void RegistersInternalExampleProvidersFromAssembly()
        {
            var services = new ServiceCollection();
            services.AddSwaggerExamplesFromAssemblyOf<PersonRequestExample>();

            using var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService(typeof(PersonRequestExample)).ShouldNotBeNull();
            serviceProvider.GetService(typeof(PersonRequestMultipleExamples)).ShouldNotBeNull();
        }
    }
}
