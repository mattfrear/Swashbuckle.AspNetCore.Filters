using Xunit;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Shouldly;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class ExamplesOperationFilterTests : BaseOperationFilterTests
    {
        private readonly IOperationFilter sut;

        public ExamplesOperationFilterTests()
        {
            var mvcJsonOptions = new MvcJsonOptions();
            var options = Options.Create(mvcJsonOptions);
            var serializerSettingsDuplicator = new SerializerSettingsDuplicator(options);

            var jsonFormatter = new JsonFormatter();
            var exampleProviderFactory = new ExamplesProviderFactory(null);

            var requestExample = new RequestExample(jsonFormatter, serializerSettingsDuplicator, exampleProviderFactory);
            var responseExample = new ResponseExample(jsonFormatter, serializerSettingsDuplicator, exampleProviderFactory);

            sut = new ExamplesOperationFilter(requestExample, responseExample);
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromMethodAttributes()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string,Response>() };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttributes));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var examples = (JObject)operation.Responses["200"].Examples;
            var actualExample = examples["application/json"];

            var expectedExample = (PersonResponse)new PersonResponseExample().GetExamples();
            actualExample["id"].ShouldBe(expectedExample.Id);
            actualExample["first"].ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromControllerAttributes()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(typeof(FakeControllers.SwaggerResponseExampleController), nameof(FakeControllers.SwaggerResponseExampleController.None));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var examples = (JObject)operation.Responses["200"].Examples;
            var actualExample = examples["application/json"];

            var expectedExample = (PersonResponse)new PersonResponseExample().GetExamples();
            actualExample["id"].ShouldBe(expectedExample.Id);
            actualExample["first"].ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void Apply_SetsRequestExamples_FromMethodAttributes()
        {
            // Arrange
            var personRequestParameter = new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/PersonRequest" } };
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { personRequestParameter } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerRequestExampleAttributes));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualSchemaExample = (JObject)filterContext.SchemaRegistry.Definitions["PersonRequest"].Example;
            var expectedExample = (PersonRequest)new PersonRequestExample().GetExamples();
            AssertPersonRequestExampleMatches(actualSchemaExample, expectedExample);

            var actualParameterExample = (JObject)personRequestParameter.Schema.Example;
            AssertPersonRequestExampleMatches(actualParameterExample, expectedExample);
        }

        private static void AssertPersonRequestExampleMatches(JObject actualSchemaExample, PersonRequest expectedExample)
        {
            actualSchemaExample["title"].ShouldBe(expectedExample.Title.ToString());
            actualSchemaExample["firstName"].ShouldBe(expectedExample.FirstName);
            actualSchemaExample["age"].ShouldBe(expectedExample.Age);
        }

        [Fact]
        public void Apply_WhenRequestIncorrect_ShouldNotThrowException()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/PersonRequest" } } } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithIncorrectSwaggerRequestExampleAttribute));

            // Act
            sut.Apply(operation, filterContext);
        }

        [Fact]
        public void Apply_WhenPassingDictionary_ShouldSetExampleOnRequestSchema()
        {
            // Arrange
            var bodyParameter = new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/object" } };
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { bodyParameter } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithDictionarySwaggerRequestExampleAttribute));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = (JObject)bodyParameter.Schema.Example;
            actualExample["PropertyInt"].ShouldBe(1);
            actualExample["PropertyString"].ShouldBe("Some string");
        }

        private void SetSwaggerResponses(Operation operation, OperationFilterContext filterContext)
        {
            var swaggerResponseFilter = new AnnotationsOperationFilter();
            swaggerResponseFilter.Apply(operation, filterContext);
        }
    }
}
