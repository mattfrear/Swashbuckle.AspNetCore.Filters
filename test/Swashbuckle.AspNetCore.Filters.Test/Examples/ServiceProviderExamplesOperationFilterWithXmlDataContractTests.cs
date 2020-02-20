using Xunit;
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
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters.Test.Extensions;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class ServiceProviderExamplesOperationFilterWithXmlDataContractTests : BaseOperationFilterTests
    {
        private readonly IOperationFilter sut;
        private readonly IServiceProvider serviceProvider;

        public ServiceProviderExamplesOperationFilterWithXmlDataContractTests()
        {
            serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IExamplesProvider<PersonResponse>)).Returns(new PersonResponseAutoExample());

            var jsonFormatter = new JsonFormatter();
            var serializerSettingsDuplicator = new SerializerSettingsDuplicator(
                Options.Create(new MvcJsonOptions()),
                Options.Create(new SchemaGeneratorOptions()));

            var mvcOutputFormatter = new MvcOutputFormatter(FormatterOptions.WithXmlDataContractFormatter, new FakeLoggerFactory());
            
            sut = new ServiceProviderExamplesOperationFilter(
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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = response.DeserializeDataContractXmlExampleAs<PersonResponse>();

            var expectedExample = new PersonResponseAutoExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
            actualExample.Age.ShouldBe(expectedExample.Age);
        }

        [Fact]
        public void Apply_SetsResponseExamples_WhenMethodNotAnnotated()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/xml", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var supportedResponseTypes = new List<ApiResponseType> { new ApiResponseType { StatusCode = 200, Type = typeof(PersonResponse) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.PersonResponseNotAnnotated), supportedResponseTypes: supportedResponseTypes);
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = response.DeserializeDataContractXmlExampleAs<PersonResponse>();

            var expectedExample = (PersonResponse)new PersonResponseAutoExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromControllerAttributes()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/xml", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var filterContext = FilterContextFor(typeof(FakeControllers.SwaggerResponseController), nameof(FakeControllers.SwaggerResponseController.None));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = response.DeserializeDataContractXmlExampleAs<PersonResponse>();

            var expectedExample = new PersonResponseAutoExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void Apply_DoesNotSetResponseExamples_FromMethodAttributes_WhenSwaggerResponseExampleAttributePresent()
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
            example.ShouldBeNull();
        }

        [Fact]
        public void Apply_SetsResponseExamples_FromMethodAttributes_WithGenericType()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/xml", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.GenericAnnotatedWithSwaggerResponseAttribute));
            SetSwaggerResponses(operation, filterContext);
            serviceProvider.GetService(typeof(IExamplesProvider<IEnumerable<string>>)).Returns(new ListStringExample());

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = response.DeserializeDataContractXmlExampleAs<List<string>>();
            actualExample[0].ShouldBe("Hello");
            actualExample[1].ShouldBe("there");
        }

        [Fact]
        public void Apply_SetsRequestExamples_FromMethodAttributes()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<PersonRequest>)).Returns(new PersonRequestAutoExample());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/xml", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(PersonRequest) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.PersonRequestUnannotated), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = requestBody.DeserializeDataContractXmlExampleAs<PersonRequest>();
            var expectedExample = new PersonRequestAutoExample().GetExamples();
            actualExample.ShouldMatch(expectedExample);
        }

        [Fact]
        public void Apply_DoesNotSetRequestExamples_FromMethodAttributes_WhenSwaggerRequestExampleAttributePresent()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<PersonRequest>)).Returns(new PersonRequestAutoExample());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/xml", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(PersonRequest) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerRequestExampleAttribute), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            requestBody.Content["application/xml"].Example.ShouldBeNull();
        }

        [Fact]
        public void Apply_SetsRequestExamplesForInterface_FromMethodAttributes()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<IPersonRequest>)).Returns(new IPersonRequestAutoExample());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/xml", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(IPersonRequest) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.IPersonRequestUnannotated), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = requestBody.DeserializeDataContractXmlExampleAs<PersonRequest>();
            var expectedExample = new PersonRequestAutoExample().GetExamples();
            actualExample.ShouldMatch(expectedExample);
        }

        [Fact]
        public void Apply_WhenRequestIsAntInt_ShouldNotThrowException()
        {
            // Arrange
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/xml", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(int) } };

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.RequestTakesAnInt), parameterDescriptions);

            // Act
            Should.NotThrow(() => sut.Apply(operation, filterContext));
        }

        [Fact]
        public void Apply_WhenRequestIsANullableEnum_ShouldNotThrowException()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<Title?>)).Returns(new TitleExample());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/xml", new OpenApiMediaType() { Schema = new OpenApiSchema { Reference = new OpenApiReference { Id = "definitions/Title" } } } }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };

            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(Title?) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.RequestTakesANullableEnum), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualParameterExample = requestBody.DeserializeDataContractXmlExampleAs<Title?>();
            var expectedExample = new TitleExample().GetExamples();
            actualParameterExample.ShouldBe(expectedExample);
        }

        [Fact]
        public void Apply_WhenPassingDictionary_ShouldSetExampleOnRequestSchema()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<Dictionary<string, object>>)).Returns(new DictionaryAutoRequestExample());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/xml", new OpenApiMediaType() { Schema = new OpenApiSchema { Reference = new OpenApiReference { Id = "definitions/object" } } } }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(Dictionary<string, object>) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.DictionaryRequestAttribute), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = requestBody.DeserializeDataContractXmlExampleAs<IDictionary<string, object>>();
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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAttribute));
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
