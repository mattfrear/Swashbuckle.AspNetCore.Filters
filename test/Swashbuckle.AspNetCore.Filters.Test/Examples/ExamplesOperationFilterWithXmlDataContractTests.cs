using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Shouldly;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using Xunit;
using Swashbuckle.AspNetCore.Filters.Test.Extensions;
using Microsoft.OpenApi.Any;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class ExamplesOperationFilterWithXmlDataContractTests : BaseOperationFilterTests
    {
        private readonly ExamplesOperationFilter sut;

        public ExamplesOperationFilterWithXmlDataContractTests()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(PersonResponseExample)).Returns(new PersonResponseExample());
            serviceProvider.GetService(typeof(PersonRequestExample)).Returns(new PersonRequestExample());
            serviceProvider.GetService(typeof(DictionaryRequestExample)).Returns(new DictionaryRequestExample());

            var jsonFormatter = new JsonFormatter();
            var serializerSettingsDuplicator = new SerializerSettingsDuplicator(
                Options.Create(new MvcJsonOptions()),
                Options.Create(new SchemaGeneratorOptions()));

            var mvcOutputFormatter = new MvcOutputFormatter(FormatterOptions.WithXmlDataContractFormatter, new FakeLoggerFactory());

            sut = new ExamplesOperationFilter(
                serviceProvider,
                new RequestExample(jsonFormatter, serializerSettingsDuplicator, mvcOutputFormatter),
                new ResponseExample(jsonFormatter, serializerSettingsDuplicator, mvcOutputFormatter));
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromMethodAttributes()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/xml", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = response.DeserializeDataContractXmlExampleAs<PersonResponse>();

            var expectedExample = new PersonResponseExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
            actualExample.Age.ShouldBe(27);
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromControllerAttributes()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/xml", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeControllers.SwaggerResponseExampleController), nameof(FakeControllers.SwaggerResponseExampleController.None));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = response.DeserializeDataContractXmlExampleAs<PersonResponse>();

            var expectedExample = new PersonResponseExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromMethodAttributesPascalCase()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/xml", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttributePascalCase));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            string xmlExample = ((OpenApiString)response.Content["application/xml"].Example).Value;
            var expectedExample = new PersonResponseExample().GetExamples();
            xmlExample.ShouldContain($"<Id>{expectedExample.Id}</Id>", Case.Sensitive);
        }

        [Fact]
        public void Apply_SetsRequestExamples_FromMethodAttributes()
        {
            // Arrange
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType> { { "application/xml", new OpenApiMediaType() } }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerRequestExampleAttribute));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = requestBody.DeserializeDataContractXmlExampleAs<PersonRequest>();
            var expectedExample = new PersonRequestExample().GetExamples();
            actualExample.ShouldMatch(expectedExample);
        }
        
        [Fact]
        public void Apply_WhenRequestIncorrect_ShouldNotThrowException()
        {
            // Arrange
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType> { { "application/xml", new OpenApiMediaType() } }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithIncorrectSwaggerRequestExampleAttribute));

            // Act
            Should.NotThrow(() => sut.Apply(operation, filterContext));
        }

        [Fact]
        public void Apply_WhenPassingDictionary_ShouldSetExampleOnRequestSchema()
        {
            // Arrange
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType> { { "application/xml", new OpenApiMediaType() } }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithDictionarySwaggerRequestExampleAttribute));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = requestBody.DeserializeDataContractXmlExampleAs<Dictionary<string, object>>();
            actualExample["PropertyInt"].ShouldBe(1);
            actualExample["PropertyString"].ShouldBe("Some string");
        }
        
        [Fact]
        public void Apply_SetsResponseExamples_CorrectlyFormatsXmlExample()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/xml", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var example = response.Content["application/xml"].Example;

            var formatedExample = RenderOpenApiObject(example);
            formatedExample.EndsWith('"').ShouldBeTrue();
            formatedExample.StartsWith('"').ShouldBeTrue();
        }
    }
}
