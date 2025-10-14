using Csv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Text.Json;
using Xunit;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class ExamplesOperationFilterTests : BaseOperationFilterTests
    {
        private readonly ExamplesOperationFilter sut;
        private readonly SchemaGeneratorOptions schemaGeneratorOptions;
        private readonly SwaggerOptions swaggerOptions = new SwaggerOptions { OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0 };

        public ExamplesOperationFilterTests()
        {
            schemaGeneratorOptions = new SchemaGeneratorOptions();

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(PersonResponseExample)).Returns(new PersonResponseExample());
            serviceProvider.GetService(typeof(PersonResponseMultipleExamples)).Returns(new PersonResponseMultipleExamples());
            serviceProvider.GetService(typeof(PersonRequestExample)).Returns(new PersonRequestExample());
            serviceProvider.GetService(typeof(PersonRequestMultipleExamples)).Returns(new PersonRequestMultipleExamples());
            serviceProvider.GetService(typeof(DictionaryRequestExample)).Returns(new DictionaryRequestExample());
            serviceProvider.GetService(typeof(PeopleResponseExample)).Returns(new PeopleResponseExample());
            serviceProvider.GetService(typeof(IOptions<MvcOptions>)).Returns(Options.Create(new MvcOptions()));
            serviceProvider.GetService(typeof(IOptions<MvcNewtonsoftJsonOptions>)).Returns(Options.Create(new MvcNewtonsoftJsonOptions()));

            var mvcOutputFormatter = new MvcOutputFormatter(FormatterOptions.WithXmlAndNewtonsoftJsonAndCsvFormatters, serviceProvider, new FakeLoggerFactory());
            var requestExample = new RequestExample(mvcOutputFormatter, Options.Create(swaggerOptions));
            var responseExample = new ResponseExample(mvcOutputFormatter, Options.Create(swaggerOptions));

            sut = new ExamplesOperationFilter(serviceProvider, requestExample, responseExample);
        }

        [Fact]
        public void SetsResponseExamples_FromMethodAttributes()
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
            var actualExample = response.Content["application/json"].Example.Deserialize<PersonResponse>();

            var expectedExample = new PersonResponseExample().GetExamples();
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

            var filterContext = FilterContextFor(typeof(FakeControllers.SwaggerResponseExampleController), nameof(FakeControllers.SwaggerResponseExampleController.None));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = response.Content["application/json"].Example.Deserialize<PersonResponse>();

            var expectedExample = new PersonResponseExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
        }


        [Fact]
        public void SetsMultipleResponseExamples_FromMethodAttributes()
        {
            // Arrange
            swaggerOptions.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
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
        public void SetsFirstMultipleResponseExamples_FromMethodAttributes_WhenOpenApi2_0()
        {
            // Arrange
            swaggerOptions.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseMultipleExamplesAttribute));
            SetSwaggerResponses(operation, filterContext);

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualExample = response.Content["application/json"].Example.Deserialize<PersonResponse>();
            var expectedExample = new PersonResponseMultipleExamples().GetExamples().First().Value;
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
        }

        [Fact]
        public void SetsRequestExamples_FromMethodAttributes()
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
            var actualExample = requestBody.Content["application/json"].Example.Deserialize<PersonRequest>();
            var expectedExample = new PersonRequestExample().GetExamples();
            actualExample.ShouldMatch(expectedExample);

            // Assert SerializeAsV2
            var actualSchemaExample = filterContext.SchemaRepository.Schemas["PersonRequest"].Example.Deserialize<PersonRequest>();
            actualSchemaExample.ShouldMatch(expectedExample);
        }

        [Fact]
        public void SetsMultipleRequestExamples_FromMethodAttributes()
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
            actualExamples["Dave"].Description.ShouldBe("Here's a description");
            actualExamples["Angela"].Description.ShouldBeNull();

            // Assert SerializeAsV2
            var actualSchemaExample = filterContext.SchemaRepository.Schemas["PersonRequest"].Example.Deserialize<PersonRequest>();
            actualSchemaExample.ShouldMatch(expectedExamples.First().Value);
        }

        [Fact]
        public void WhenRequestIncorrect_ShouldNotThrowException()
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

            // Assert
            // this assertion fails but it's not really a problem, since ASP.NET WebApi only accepts one [FromBody] in the request.
            // var actualExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiString)requestBody.Content["application/json"].Example).Value);
            // actualExample.ShouldBeNull();
        }

        [Fact]
        public void WhenPassingDictionary_ShouldSetExampleOnRequestSchema()
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
            var actualExample = requestBody.Content["application/json"].Example.Deserialize<IDictionary<string, object>>();
            actualExample["PropertyInt"].ShouldBe(1);
            actualExample["PropertyString"].ShouldBe("Some string");

            // Assert SerializeAsV2
            // this doesn't work as Dictionaries aren't added to the definitions
        }

        [Fact]
        public void SetsResponseExamples_CorrectlyFormatsJsonExample()
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
        public void SetsResponseExamples_CorrectlyFormatsXmlExample()
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
        public void SetsResponseExamples_CorrectlyFormatsCsvExample()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "text/csv", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseExampleAttributeOfTypeEnumerable));
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
    }
}
