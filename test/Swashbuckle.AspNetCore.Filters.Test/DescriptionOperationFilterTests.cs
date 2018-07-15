using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json.Linq;
using Shouldly;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using Xunit;

namespace Swashbuckle.AspNetCore.Filters.Test
{
    public class DescriptionOperationFilterTests : BaseOperationFilterTests
    {
        private readonly IOperationFilter sut;

        public DescriptionOperationFilterTests()
        {
            sut = new DescriptionOperationFilter();
        }


        [Fact]
        public void Apply_SetsResponseDescriptions()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttributes));
            SetSwaggerResponses(operation, filterContext);
            filterContext.SchemaRegistry.GetOrRegister(typeof(PersonResponse));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var schema = filterContext.SchemaRegistry.Definitions["PersonResponse"];
            schema.Properties["first"].Description.ShouldBe("The first name of the person");
            schema.Properties["last"].Description.ShouldBe("The last name of the person");
        }

        [Fact]
        public void Apply_SetsRequestDescriptions()
        {
            // Arrange
            var operation = new Operation
            {
                OperationId = "foobar",
                Responses = new Dictionary<string, Response>()
            };

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerRequestExampleAttributes));
            SetSwaggerResponses(operation, filterContext);
            filterContext.SchemaRegistry.GetOrRegister(typeof(PersonRequest));
            filterContext.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription { Type = typeof(PersonRequest), Name = nameof(PersonRequest) });

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var schema = filterContext.SchemaRegistry.Definitions["PersonRequest"];
            schema.Properties["firstName"].Description.ShouldBe("The first name of the person");
        }
    }
}
