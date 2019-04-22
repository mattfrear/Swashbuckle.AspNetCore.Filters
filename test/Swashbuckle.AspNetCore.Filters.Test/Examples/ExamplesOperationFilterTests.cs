using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NSubstitute;
using Shouldly;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using Xunit;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class ExamplesOperationFilterTests : BaseOperationFilterTests
    {
        private readonly IOperationFilter sut;

        public ExamplesOperationFilterTests()
        {
            var options = Options.Create(new MvcJsonOptions());
            var serializerSettingsDuplicator = new SerializerSettingsDuplicator(options);

            var jsonFormatter = new JsonFormatter();

            var serviceProvider = Substitute.For<IServiceProvider>();

            var requestExample = new RequestExample(jsonFormatter, serializerSettingsDuplicator);
            var responseExample = new ResponseExample(jsonFormatter, serializerSettingsDuplicator);

            sut = new ExamplesOperationFilter(serviceProvider, requestExample, responseExample);
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
            var actualExample = JsonConvert.DeserializeObject<PersonResponse>(((OpenApiString)response.Content["application/json"].Example).Value);

            var expectedExample = (PersonResponse)new PersonResponseExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
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
            var actualExample = JsonConvert.DeserializeObject<PersonResponse>(((OpenApiString)response.Content["application/json"].Example).Value);

            var expectedExample = (PersonResponse)new PersonResponseExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
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
            var actualExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiString)requestBody.Content["application/json"].Example).Value);
            var expectedExample = (PersonRequest)new PersonRequestExample().GetExamples();
            AssertPersonRequestExampleMatches(actualExample, expectedExample);
        }

        private static void AssertPersonRequestExampleMatches(PersonRequest actualExample, PersonRequest expectedExample)
        {
            actualExample.Title.ShouldBe(expectedExample.Title);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
            actualExample.Age.ShouldBe(expectedExample.Age);
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
            var actualExample = JsonConvert.DeserializeObject<Dictionary<string, object>>(((OpenApiString)requestBody.Content["application/json"].Example).Value);
            actualExample["PropertyInt"].ShouldBe(1);
            actualExample["PropertyString"].ShouldBe("Some string");
        }
    }
}
