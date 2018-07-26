using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json.Linq;
using Shouldly;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using Xunit;
using System;
using Newtonsoft.Json.Serialization;

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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttribute));
            SetSwaggerResponses(operation, filterContext);
            filterContext.SchemaRegistry.GetOrRegister(typeof(PersonResponse));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var schema = filterContext.SchemaRegistry.Definitions["PersonResponse"];
            schema.Properties["first"].Description.ShouldBe("The first name of the person");
            schema.Properties["last"].Description.ShouldBe("The last name of the person");
            schema.Properties["age"].Description.ShouldBe("His age, in years");
        }

        [Fact]
        public void Apply_SetsResponseDescriptionsWithDefaultContractResolver()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(
                typeof(FakeActions),
                nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttribute),
                new DefaultContractResolver());

            SetSwaggerResponses(operation, filterContext);
            filterContext.SchemaRegistry.GetOrRegister(typeof(PersonResponse));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var schema = filterContext.SchemaRegistry.Definitions["PersonResponse"];
            schema.Properties["Age"].Description.ShouldBe("His age, in years");
        }

        [Fact]
        public void Apply_SetsResponseDescriptions_WhenUsingFullname()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttribute));
            SetSwaggerResponses(operation, filterContext);

            var fullName = RegisterFullNameInSchemaRegistry(filterContext.SchemaRegistry, typeof(PersonResponse));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var schema = filterContext.SchemaRegistry.Definitions[fullName];
            schema.Properties["first"].Description.ShouldBe("The first name of the person");
            schema.Properties["last"].Description.ShouldBe("The last name of the person");
            schema.Properties["age"].Description.ShouldBe("His age, in years");
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

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerRequestExampleAttribute));
            SetSwaggerResponses(operation, filterContext);
            filterContext.SchemaRegistry.GetOrRegister(typeof(PersonRequest));
            filterContext.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription { Type = typeof(PersonRequest), Name = nameof(PersonRequest) });
            
            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var schema = filterContext.SchemaRegistry.Definitions["PersonRequest"];
            schema.Properties["firstName"].Description.ShouldBe("The first name of the person");
        }

        [Fact]
        public void Apply_SetsRequestDescriptions_WhenArray()
        {
            // Arrange
            var operation = new Operation
            {
                OperationId = "foobar",
                Responses = new Dictionary<string, Response>()
            };

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerRequestExampleAttribute));
            SetSwaggerResponses(operation, filterContext);
            filterContext.SchemaRegistry.GetOrRegister(typeof(PersonRequest));
            filterContext.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription { Type = typeof(PersonRequest), Name = nameof(PersonRequest) });

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var schema = filterContext.SchemaRegistry.Definitions["PersonRequest"];
            schema.Properties["children"].Description.ShouldBe("The person's children");

            schema = filterContext.SchemaRegistry.Definitions["Child"];
            schema.Properties["name"].Description.ShouldBe("The child's full name");
        }

        [Fact]
        public void Apply_SetsRequestDescriptions_WhenUsingFullname()
        {
            // Arrange
            var operation = new Operation
            {
                OperationId = "foobar",
                Responses = new Dictionary<string, Response>()
            };

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerRequestExampleAttribute));
            SetSwaggerResponses(operation, filterContext);
            var fullName = RegisterFullNameInSchemaRegistry(filterContext.SchemaRegistry, typeof(PersonRequest));
            filterContext.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription { Type = typeof(PersonRequest), Name = nameof(PersonRequest) });

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var schema = filterContext.SchemaRegistry.Definitions[fullName];
            schema.Properties["firstName"].Description.ShouldBe("The first name of the person");
        }

        private static string RegisterFullNameInSchemaRegistry(ISchemaRegistry schemaRegistry, Type type)
        {
            var shortName = type.FriendlyId(false);
            var fullName = type.FriendlyId(true);

            schemaRegistry.GetOrRegister(type);
            schemaRegistry.Definitions.Add(fullName, schemaRegistry.Definitions[shortName]);
            schemaRegistry.Definitions.Remove(shortName);

            return fullName;
        }
    }
}
