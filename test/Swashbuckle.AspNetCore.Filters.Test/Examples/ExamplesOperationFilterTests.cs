using Xunit;
using Swashbuckle.AspNetCore.Swagger;
using Newtonsoft.Json.Linq;
using Shouldly;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using NSubstitute;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;

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
            operation.Responses.Add("200", response );

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var examples = operation.Responses["200"].Content;
            var value = (OpenApiString)examples["application/json"].Example;
            var actualExample = JsonConvert.DeserializeObject<PersonResponse>(value.Value);

            var expectedExample = (PersonResponse)new PersonResponseExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
        }

        //[Fact]
        //public void Apply_SetsResponseExamples_FromControllerAttributes()
        //{
        //    // Arrange
        //    var operation = new Operation { Summary = "Test summary", Responses = new Dictionary<string, Response>() };
        //    var filterContext = FilterContextFor(typeof(FakeControllers.SwaggerResponseExampleController), nameof(FakeControllers.SwaggerResponseExampleController.None));
        //    SetSwaggerResponses(operation, filterContext);

        //    // Act
        //    sut.Apply(operation, filterContext);

        //    // Assert
        //    var examples = (JObject)operation.Responses["200"].Examples;
        //    var actualExample = examples["application/json"];

        //    var expectedExample = (PersonResponse)new PersonResponseExample().GetExamples();
        //    actualExample["id"].ShouldBe(expectedExample.Id);
        //    actualExample["first"].ShouldBe(expectedExample.FirstName);
        //}

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
