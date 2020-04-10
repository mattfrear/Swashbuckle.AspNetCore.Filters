using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NSubstitute;
using Shouldly;
using Swashbuckle.AspNetCore.Filters.Test.Extensions;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using Xunit;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class ExamplesOperationFilterTests : BaseOperationFilterTests
    {
        private readonly ExamplesOperationFilter sut;
        private SchemaGeneratorOptions schemaGeneratorOptions;

        public ExamplesOperationFilterTests()
        {
            var mvcJsonOptions = Options.Create(new MvcJsonOptions());
            schemaGeneratorOptions = new SchemaGeneratorOptions();
            var serializerSettingsDuplicator = new SerializerSettingsDuplicator(mvcJsonOptions, Options.Create(schemaGeneratorOptions));

            var jsonFormatter = new JsonFormatter();
            var mvcOutputFormatter = new MvcOutputFormatter(FormatterOptions.WithoutFormatters, new FakeLoggerFactory());

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(PersonResponseExample)).Returns(new PersonResponseExample());
            serviceProvider.GetService(typeof(PersonResponseMultipleExamples)).Returns(new PersonResponseMultipleExamples());
            serviceProvider.GetService(typeof(PersonRequestExample)).Returns(new PersonRequestExample());
            serviceProvider.GetService(typeof(PersonRequestMultipleExamples)).Returns(new PersonRequestMultipleExamples());
            serviceProvider.GetService(typeof(DictionaryRequestExample)).Returns(new DictionaryRequestExample());

            var requestExample = new RequestExample(jsonFormatter, serializerSettingsDuplicator, mvcOutputFormatter);
            var responseExample = new ResponseExample(jsonFormatter, serializerSettingsDuplicator, mvcOutputFormatter);

            sut = new ExamplesOperationFilter(serviceProvider, requestExample, responseExample, Options.Create(new SwaggerOptions()));
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromMethodAttributes()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<PersonResponse>(((OpenApiRawString)response.Content["application/json"].Example).Value);

            var expectedExample = new PersonResponseExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
            actualExample.Age.ShouldBe(27);
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromControllerAttributes()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeControllers.SwaggerResponseExampleController), nameof(FakeControllers.SwaggerResponseExampleController.None));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<PersonResponse>(((OpenApiRawString)response.Content["application/json"].Example).Value);

            var expectedExample = new PersonResponseExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromMethodAttributesPascalCase()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttributePascalCase));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            string jsonExample = ((OpenApiRawString)response.Content["application/json"].Example).Value;
            var expectedExample = new PersonResponseExample().GetExamples();
            jsonExample.ShouldContain($"\"Id\": {expectedExample.Id}", Case.Sensitive);
        }

        [Fact]
        public void Apply_SetsMultipleResponseExamples_FromMethodAttributes()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseMultipleExamplesAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExamples = response.Content["application/json"].Examples;
            var expectedExamples = new PersonResponseMultipleExamples().GetExamples();
            actualExamples.ShouldAllMatch(expectedExamples, ExampleAssertExtensions.ShouldMatch);
        }

        [Fact]
        public void Apply_SetsRequestExamples_FromMethodAttributes()
        {
            // Arrange
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerRequestExampleAttribute));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiRawString)requestBody.Content["application/json"].Example).Value);
            var expectedExample = new PersonRequestExample().GetExamples();
            actualExample.ShouldMatch(expectedExample);
        }

        [Fact]
        public void Apply_SetsMultipleRequestExamples_FromMethodAttributes()
        {
            // Arrange
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerRequestMultipleExamplesAttribute));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExamples = requestBody.Content["application/json"].Examples;
            var expectedExamples = new PersonRequestMultipleExamples().GetExamples();
            actualExamples.ShouldAllMatch(expectedExamples, ExampleAssertExtensions.ShouldMatch);
        }

        [Fact]
        public void Apply_WhenRequestIncorrect_ShouldNotThrowException()
        {
            // Arrange
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithIncorrectSwaggerRequestExampleAttribute));

            // Act
            sut.Apply(operation, filterContext);
        }

        [Fact]
        public void Apply_WhenPassingDictionary_ShouldSetExampleOnRequestSchema()
        {
            // Arrange
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithDictionarySwaggerRequestExampleAttribute));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<Dictionary<string, object>>(((OpenApiRawString)requestBody.Content["application/json"].Example).Value);
            actualExample["PropertyInt"].ShouldBe(1);
            actualExample["PropertyString"].ShouldBe("Some string");
        }

        [Fact]
        public void Apply_SetsResponseExamples_CorrectlyFormatsJsonExample()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var example = response.Content["application/json"].Example;

            var formatedExample = RenderOpenApiObject(example);
            formatedExample.EndsWith('"').ShouldBeFalse();
            formatedExample.StartsWith('"').ShouldBeFalse();
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

        [Fact]
        public void Apply_ShouldNotEmitObsoleteProperties()
        {
            // Arrange
            schemaGeneratorOptions.IgnoreObsoleteProperties = true;
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            string jsonExample = ((OpenApiRawString)response.Content["application/json"].Example).Value;
            var expectedExample = new PersonResponseExample().GetExamples();
            jsonExample.ShouldNotContain($"\"age\": {expectedExample.Age}", Case.Sensitive);
            jsonExample.ShouldContain($"\"id\": {expectedExample.Id}", Case.Sensitive);
        }
    }
}
