using Xunit;
using Swashbuckle.AspNetCore.Swagger;
using Newtonsoft.Json.Linq;
using Shouldly;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using Microsoft.AspNetCore.Mvc;
using System;
using NSubstitute;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class ServiceProviderExamplesOperationFilterTests : BaseOperationFilterTests
    {
        private readonly IOperationFilter sut;
        private readonly IServiceProvider serviceProvider;

        public ServiceProviderExamplesOperationFilterTests()
        {
            var options = Options.Create(new MvcJsonOptions());
            var serializerSettingsDuplicator = new SerializerSettingsDuplicator(options);

            var jsonFormatter = new JsonFormatter();

            var requestExample = new RequestExample(jsonFormatter, serializerSettingsDuplicator);
            var responseExample = new ResponseExample(jsonFormatter, serializerSettingsDuplicator);

            serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IExamplesProvider<PersonResponse>)).Returns(new PersonResponseAutoExample());

            sut = new ServiceProviderExamplesOperationFilter(serviceProvider, requestExample, responseExample);
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromMethodAttributes()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string,Response>() };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var examples = (JObject)operation.Responses["200"].Examples;
            var actualExample = examples["application/json"];

            var expectedExample = (PersonResponse)new PersonResponseAutoExample().GetExamples();
            actualExample["id"].ShouldBe(expectedExample.Id);
            actualExample["first"].ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void Apply_SetsResponseExamples_WhenMethodNotAnnotated()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string, Response>() { { "200", new Response() } } };
            var supportedResponseTypes = new List<ApiResponseType> { new ApiResponseType { StatusCode = 200, Type = typeof(PersonResponse) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.PersonResponseNotAnnotated), supportedResponseTypes: supportedResponseTypes);
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var examples = (JObject)operation.Responses["200"].Examples;
            var actualExample = examples["application/json"];

            var expectedExample = (PersonResponse)new PersonResponseAutoExample().GetExamples();
            actualExample["id"].ShouldBe(expectedExample.Id);
            actualExample["first"].ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromControllerAttributes()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(typeof(FakeControllers.SwaggerResponseController), nameof(FakeControllers.SwaggerResponseController.None));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var examples = (JObject)operation.Responses["200"].Examples;
            var actualExample = examples["application/json"];

            var expectedExample = (PersonResponse)new PersonResponseAutoExample().GetExamples();
            actualExample["id"].ShouldBe(expectedExample.Id);
            actualExample["first"].ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void Apply_DoesNotSetResponseExamples_FromMethodAttributes_WhenSwaggerResponseExampleAttributePresent()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var examples = (JObject)operation.Responses["200"].Examples;
            examples.ShouldBeNull();
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromMethodAttributes_WithGenericType()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.GenericAnnotatedWithSwaggerResponseAttribute));
            SetSwaggerResponses(operation, filterContext);
            serviceProvider.GetService(typeof(IExamplesProvider<IEnumerable<string>>)).Returns(new ListStringExample());

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var examples = (JObject)operation.Responses["200"].Examples;
            var actualExample = examples["application/json"];

            actualExample[0].ShouldBe("Hello");
            actualExample[1].ShouldBe("there");
        }

        [Fact]
        public void Apply_SetsRequestExamples_FromMethodAttributes()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<PersonRequest>)).Returns(new PersonRequestAutoExample());
            var personRequestParameter = new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/PersonRequest" } };
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { personRequestParameter } };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(PersonRequest) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.PersonRequestUnannotated), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualSchemaExample = (JObject)filterContext.SchemaRegistry.Definitions["PersonRequest"].Example;
            var expectedExample = (PersonRequest)new PersonRequestAutoExample().GetExamples();
            AssertPersonRequestExampleMatches(actualSchemaExample, expectedExample);
        }

        [Fact]
        public void Apply_DoesNotSetRequestExamples_FromMethodAttributes_WhenSwaggerRequestExampleAttributePresent()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<PersonRequest>)).Returns(new PersonRequestAutoExample());
            var personRequestParameter = new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/PersonRequest" } };
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { personRequestParameter } };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(PersonRequest) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerRequestExampleAttribute), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            filterContext.SchemaRegistry.Definitions.ShouldNotContainKey("PersonRequest");

            personRequestParameter.Schema.Example.ShouldBeNull();
        }

        [Fact]
        public void Apply_SetsRequestExamplesForInterface_FromMethodAttributes()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<IPersonRequest>)).Returns(new IPersonRequestAutoExample());
            var personRequestParameter = new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/IPersonRequest" } };
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { personRequestParameter } };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(IPersonRequest) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.IPersonRequestUnannotated), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualSchemaExample = (JObject)filterContext.SchemaRegistry.Definitions["IPersonRequest"].Example;
            var expectedExample = (PersonRequest)new PersonRequestAutoExample().GetExamples();
            AssertPersonRequestExampleMatches(actualSchemaExample, expectedExample);
        }

        private static void AssertPersonRequestExampleMatches(JObject actualSchemaExample, PersonRequest expectedExample)
        {
            actualSchemaExample["firstName"].ShouldBe(expectedExample.FirstName);
            actualSchemaExample["age"].ShouldBe(expectedExample.Age);
        }

        [Fact]
        public void Apply_WhenRequestIsAntInt_ShouldNotThrowException()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/PersonRequest" } } } };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(int) } };

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.RequestTakesAnInt), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);
        }

        [Fact]
        public void Apply_WhenRequestIsANullableEnum_ShouldNotThrowException()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<Title?>)).Returns(new TitleExample());
            var titleParameter = new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/Title" } };
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { titleParameter } };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(Title?) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.RequestTakesANullableEnum), parameterDescriptions);
            
            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualParameterExample = Enum.Parse(typeof(Title), titleParameter.Schema.Example.ToString());
            var expectedExample = new TitleExample().GetExamples().Value;
            actualParameterExample.ShouldBe(expectedExample);
        }

        [Fact]
        public void Apply_WhenPassingDictionary_ShouldSetExampleOnRequestSchema()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<Dictionary<string, object>>)).Returns(new DictionaryAutoRequestExample());
            var bodyParameter = new BodyParameter { In = "body", Schema = new Schema { Ref = "#/definitions/object" } };
            var operation = new Operation { OperationId = "foobar", Parameters = new[] { bodyParameter } };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(Dictionary<string, object>) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.DictionaryRequestAttribute), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = (JObject)bodyParameter.Schema.Example;
            actualExample["PropertyInt"].ShouldBe(1);
            actualExample["PropertyString"].ShouldBe("Some string");
        }
    }
}
