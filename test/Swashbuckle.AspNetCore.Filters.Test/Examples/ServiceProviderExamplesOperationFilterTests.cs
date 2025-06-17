using Csv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
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
using System.Linq;
using Xunit;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class ServiceProviderExamplesOperationFilterTests : BaseOperationFilterTests
    {
        private readonly IOperationFilter sut;
        private readonly IServiceProvider serviceProvider;
        private readonly SchemaGeneratorOptions schemaGeneratorOptions;
        private readonly SwaggerOptions swaggerOptions = new SwaggerOptions { OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0 };

        public ServiceProviderExamplesOperationFilterTests()
        {
            schemaGeneratorOptions = new SchemaGeneratorOptions();

            serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IExamplesProvider<PersonResponse>)).Returns(new PersonResponseAutoExample());
            serviceProvider.GetService(typeof(IExamplesProvider<IEnumerable<PersonResponse>>)).Returns(new PeopleResponseExample());
            serviceProvider.GetService(typeof(IOptions<MvcOptions>)).Returns(Options.Create(new MvcOptions()));
            serviceProvider.GetService(typeof(IOptions<MvcNewtonsoftJsonOptions>)).Returns(Options.Create(new MvcNewtonsoftJsonOptions()));

            var mvcOutputFormatter = new MvcOutputFormatter(FormatterOptions.WithXmlAndNewtonsoftJsonAndCsvFormatters, serviceProvider, new FakeLoggerFactory());

            var requestExample = new RequestExample(mvcOutputFormatter, Options.Create(swaggerOptions));
            var responseExample = new ResponseExample(mvcOutputFormatter, Options.Create(swaggerOptions));

            sut = new ServiceProviderExamplesOperationFilter(serviceProvider, requestExample, responseExample);
        }

        [Fact]
        public void SetsResponseExamples_FromMethodAttributes()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<PersonResponse>(((OpenApiString)response.Content["application/json"].Example).Value);

            var expectedExample = new PersonResponseAutoExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void SetsResponseExamples_WhenMethodNotAnnotated()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var supportedResponseTypes = new List<ApiResponseType> { new ApiResponseType { StatusCode = 200, Type = typeof(PersonResponse) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.PersonResponseNotAnnotated), supportedResponseTypes: supportedResponseTypes);
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<PersonResponse>(((OpenApiString)response.Content["application/json"].Example).Value);

            var expectedExample = new PersonResponseAutoExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void SetsResponseExamples_FromControllerAttributes()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var filterContext = FilterContextFor(typeof(FakeControllers.SwaggerResponseController), nameof(FakeControllers.SwaggerResponseController.None));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<PersonResponse>(((OpenApiString)response.Content["application/json"].Example).Value);

            var expectedExample = new PersonResponseAutoExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void DoesNotSetResponseExamples_FromMethodAttributes_WhenSwaggerResponseExampleAttributePresent()
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
            example.ShouldBeNull();
        }

        [Fact]
        public void SetsResponseExamples_FromMethodAttributes_WithGenericType()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.GenericAnnotatedWithSwaggerResponseAttribute));
            SetSwaggerResponses(operation, filterContext);
            serviceProvider.GetService(typeof(IExamplesProvider<IEnumerable<string>>)).Returns(new ListStringExample());

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<List<string>>(((OpenApiString)response.Content["application/json"].Example).Value);
            actualExample[0].ShouldBe("Hello");
            actualExample[1].ShouldBe("there");
        }

        [Fact]
        public void SetsRequestExamples_FromMethodParameters()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<PersonRequest>)).Returns(new PersonRequestAutoExample());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(PersonRequest), Source = BindingSource.Body } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.PersonRequestUnannotated), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiString)requestBody.Content["application/json"].Example).Value);
            var expectedExample = new PersonRequestAutoExample().GetExamples();
            actualExample.ShouldMatch(expectedExample);

            // Assert SerializeAsV2
            var actualSchemaExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiString)filterContext.SchemaRepository.Schemas["PersonRequest"].Example).Value);
            actualSchemaExample.ShouldMatch(expectedExample);
        }

        [Fact]
        public void DoesNotSetRequestExamples_FromMethodAttributes_WhenSwaggerRequestExampleAttributePresent()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<PersonRequest>)).Returns(new PersonRequestAutoExample());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(PersonRequest) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerRequestExampleAttribute), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            requestBody.Content["application/json"].Example.ShouldBeNull();
        }

        [Fact]
        public void SetsRequestExamplesForInterface_FromMethodAttributes()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<IPersonRequest>)).Returns(new IPersonRequestAutoExample());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(IPersonRequest), Source = BindingSource.Body } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.IPersonRequestUnannotated), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiString)requestBody.Content["application/json"].Example).Value);
            var expectedExample = new PersonRequestAutoExample().GetExamples();
            actualExample.ShouldMatch(expectedExample);

            // Assert SerializeAsV2
            var actualSchemaExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiString)filterContext.SchemaRepository.Schemas["IPersonRequest"].Example).Value);
            actualSchemaExample.ShouldMatch(expectedExample);
        }

        [Fact]
        public void WhenRequestIsAntInt_ShouldNotThrowException()
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
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(int) } };

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.RequestTakesAnInt), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);
        }

        [Fact]
        public void WhenRequestIsANullableEnum_ShouldNotThrowException()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<Title?>)).Returns(new TitleExample());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() { Schema = new OpenApiSchema { Reference = new OpenApiReference { Id = "definitions/Title" } } } }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };

            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(Title?), Source = BindingSource.Body } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.RequestTakesANullableEnum), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualParameterExample = Enum.Parse(typeof(Title), ((OpenApiString)requestBody.Content["application/json"].Example).Value);
            var expectedExample = new TitleExample().GetExamples().Value;
            actualParameterExample.ShouldBe(expectedExample);

            // Assert SerializeAsV2
            var actualSchemaExample = JsonConvert.DeserializeObject<Title>(((OpenApiString)filterContext.SchemaRepository.Schemas["Title"].Example).Value);
            actualSchemaExample.ShouldBe(expectedExample);
        }

        [Fact]
        public void SetsMultipleRequestExamples()
        {
            // Arrange
            serviceProvider.GetService(typeof(IMultipleExamplesProvider<PersonRequest>)).Returns(new PersonRequestMultipleExamples());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(PersonRequest), Source = BindingSource.Body } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.PersonRequestUnannotated), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExamples = requestBody.Content["application/json"].Examples;
            var expectedExamples = new PersonRequestMultipleExamples().GetExamples();
            actualExamples.ShouldAllMatch(expectedExamples, ExampleAssertExtensions.ShouldMatch);
            actualExamples["Dave"].Description.ShouldBe("Here's a description");
            actualExamples["Angela"].Description.ShouldBeNull();

            // Assert SerializeAsV2
            var actualSchemaExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiString)filterContext.SchemaRepository.Schemas["PersonRequest"].Example).Value);
            actualSchemaExample.ShouldMatch(expectedExamples.First().Value);
        }

        [Fact]
        public void WhenMultipleRequestsExamplesIsEmpty_ShouldSetNoRequests()
        {
            // Arrange
            serviceProvider.GetService(typeof(IMultipleExamplesProvider<PersonRequest>)).Returns(new PersonRequestMultipleExamplesEmpty());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(PersonRequest) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.PersonRequestUnannotated), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExamples = requestBody.Content["application/json"].Examples;
            actualExamples.Count.ShouldBe(0);
        }

        [Fact]
        public void WhenMultipleRequestsExamplesKeysAreDuplicated_ShouldNotThrow()
        {
            // Arrange
            serviceProvider.GetService(typeof(IMultipleExamplesProvider<PersonRequest>)).Returns(new PersonRequestMultipleExamplesDuplicatedKeys());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(PersonRequest), Source = BindingSource.Body } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.PersonRequestUnannotated), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExamples = requestBody.Content["application/json"].Examples;
            var expectedExamples = new PersonRequestMultipleExamplesDuplicatedKeys()
                .GetExamples()
                .GroupBy(ex => ex.Name)
                .Select(ex => ex.First())
                .ToList();
            actualExamples.ShouldAllMatch(expectedExamples, ExampleAssertExtensions.ShouldMatch);
        }

        [Fact]
        public void WhenMultipleRequestsExamplesReturnsNull_ShouldNotThrow()
        {
            // Arrange
            serviceProvider.GetService(typeof(IMultipleExamplesProvider<PersonRequest>)).Returns(new PersonRequestMultipleExamplesNull());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(PersonRequest) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.PersonRequestUnannotated), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExamples = requestBody.Content["application/json"].Examples;
            actualExamples.Count.ShouldBe(0);
        }

        [Fact]
        public void WhenPassingDictionary_ShouldSetExampleOnRequestSchema()
        {
            // Arrange
            serviceProvider.GetService(typeof(IExamplesProvider<Dictionary<string, object>>)).Returns(new DictionaryAutoRequestExample());
            var requestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    { "application/json", new OpenApiMediaType() { Schema = new OpenApiSchema { Reference = new OpenApiReference { Id = "definitions/object" } } } }
                }
            };
            var operation = new OpenApiOperation { OperationId = "foobar", RequestBody = requestBody };
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(Dictionary<string, object>), Source = BindingSource.Body } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.DictionaryRequestAttribute), parameterDescriptions);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<Dictionary<string, object>>(((OpenApiString)requestBody.Content["application/json"].Example).Value);
            actualExample["PropertyInt"].ShouldBe(1);
            actualExample["PropertyString"].ShouldBe("Some string");
        }

        [Fact]
        public void SetsResponseExamples_CorrectlyFormatsJsonExample()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAttribute));
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
        public void SetsResponseExamples_CorrectlyFormatsXmlExample()
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
            formatedExample.Contains("<FirstName>").ShouldBeTrue();
        }

        [Fact]
        public void SetsResponseExamples_CorrectlyFormatsCsvExample()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "text/csv", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAttributeOfTypeEnumerable));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var example = response.Content["text/csv"].Example;

            var formatedExample = RenderOpenApiObject(example);
            formatedExample.EndsWith('"').ShouldBeTrue();
            formatedExample.StartsWith('"').ShouldBeTrue();

            IEnumerable<ICsvLine> lines = CsvReader.ReadFromText(
                formatedExample.Trim('"').Replace("\\r\\n", Environment.NewLine),
                new CsvOptions { Separator = ';' });

            lines.ShouldContain(x => x["FirstName"] == "John" && x["last"] == "Doe");
            lines.ShouldContain(x => x["FirstName"] == "Jane" && x["last"] == "Smith");
        }

        [Fact]
        public void ShouldEmitSystemTextJsonPropertyName()
        {
            // Arrange
            var mvcOutputFormatter = new MvcOutputFormatter(FormatterOptions.WithSystemTextJsonFormatter, serviceProvider, new FakeLoggerFactory());
            var responseExample = new ResponseExample(mvcOutputFormatter, Options.Create(swaggerOptions));
            var sut = new ServiceProviderExamplesOperationFilter(serviceProvider, null, responseExample);

            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            string jsonExample = ((OpenApiString)response.Content["application/json"].Example).Value;
            var expectedExample = new PersonResponseAutoExample().GetExamples();
            jsonExample.ShouldContain($"\"lastagain\": \"{expectedExample.LastName}\"", Case.Sensitive);
        }

        [Fact]
        public void ShouldSetExampleForDefaultResponse()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("default", response);
            var supportedResponseTypes = new List<ApiResponseType> { new ApiResponseType { StatusCode = 0, Type = typeof(PersonResponse) } };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.PersonResponseNotAnnotated), supportedResponseTypes: supportedResponseTypes);
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = JsonConvert.DeserializeObject<PersonResponse>(((OpenApiString)response.Content["application/json"].Example).Value);

            var expectedExample = new PersonResponseExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
        }
    }
}
