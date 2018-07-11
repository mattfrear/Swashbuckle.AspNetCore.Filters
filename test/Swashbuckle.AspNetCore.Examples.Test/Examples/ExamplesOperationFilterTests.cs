using System.Linq;
using Newtonsoft.Json;
using Xunit;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.Examples;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Shouldly;
using Swashbuckle.AspNetCore.Examples.Test.TestFixtures.Fakes.Examples;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;
using System;
using System.Diagnostics;

namespace Swashbuckle.AspNetCore.SwaggerGen.Test
{
    public class ExamplesOperationFilterTests
    {
        private readonly IOperationFilter sut;

        public ExamplesOperationFilterTests()
        {
            var mvcJsonOptions = new MvcJsonOptions();
            var options = Options.Create(mvcJsonOptions);
            sut = new ExamplesOperationFilter(options);
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromMethodAttributes()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string,Response>() };
            var filterContext = FilterContextFor(nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttributes));
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
            var filterContext = FilterContextFor(nameof(FakeActions.None), nameof(FakeControllers.SwaggerResponseExampleController), typeof(FakeControllers.SwaggerResponseExampleController));
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
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/PersonRequest" } } } };
            var filterContext = FilterContextFor(nameof(FakeActions.AnnotatedWithSwaggerRequestExampleAttributes));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = (JObject)filterContext.SchemaRegistry.Definitions["PersonRequest"].Example;
            var expectedExample = (PersonRequest)new PersonRequestExample().GetExamples();
            actualExample["title"].ShouldBe(expectedExample.Title.ToString());
            actualExample["firstName"].ShouldBe(expectedExample.FirstName);
            actualExample["age"].ShouldBe(expectedExample.Age);
        }

        [Fact]
        public void Apply_WhenRequestIncorrect_ShouldNotThrowException()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/PersonRequest" } } } };
            var filterContext = FilterContextFor(nameof(FakeActions.AnnotatedWithIncorrectSwaggerRequestExampleAttribute));

            // Act
            sut.Apply(operation, filterContext);
        }

        [Fact]
        public void Apply_WhenPassingDictionary_ShouldSetExampleOnRequestSchema()
        {
            // Arrange
            var bodyParameter = new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/object" } };
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { bodyParameter } };
            var filterContext = FilterContextFor(nameof(FakeActions.AnnotatedWithDictionarySwaggerRequestExampleAttribute));

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
            while (false == Debugger.IsAttached) { }
            swaggerResponseFilter.Apply(operation, filterContext);
        }

        private OperationFilterContext FilterContextFor(
            string actionFixtureName,
            string controllerFixtureName = "NotAnnotated",
            Type controllerType = null)
        {
            var fakeProvider = new FakeApiDescriptionGroupCollectionProvider();
            var apiDescription = fakeProvider
                .Add("GET", "collection", actionFixtureName, controllerFixtureName)
                .ApiDescriptionGroups.Items.First()
                .Items.First();

            var mi = (controllerType ?? typeof(FakeActions)).GetMethod(actionFixtureName);

            return new OperationFilterContext(
                apiDescription,
                new SchemaRegistry(new JsonSerializerSettings()),
                mi);
        }
    }
}
