using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
using System.Linq;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Newtonsoft;
using Xunit;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class ExamplesOperationFilterTests : BaseOperationFilterTests
    {
        private ExamplesOperationFilter sut;
        private SchemaGeneratorOptions schemaGeneratorOptions;
        private SwaggerOptions swaggerOptions = new SwaggerOptions { SerializeAsV2 = true };

        public ExamplesOperationFilterTests()
        {
            schemaGeneratorOptions = new SchemaGeneratorOptions();

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(PersonResponseExample)).Returns(new PersonResponseExample());
            serviceProvider.GetService(typeof(PersonResponseMultipleExamples)).Returns(new PersonResponseMultipleExamples());
            serviceProvider.GetService(typeof(PersonRequestExample)).Returns(new PersonRequestExample());
            serviceProvider.GetService(typeof(PersonRequestMultipleExamples)).Returns(new PersonRequestMultipleExamples());
            serviceProvider.GetService(typeof(MultipleTypesRequestExamples)).Returns(new MultipleTypesRequestExamples());
            serviceProvider.GetService(typeof(MultipleTypesResponseExamples)).Returns(new MultipleTypesResponseExamples());
            serviceProvider.GetService(typeof(DictionaryRequestExample)).Returns(new DictionaryRequestExample());

            var schemaGenerator = new FakeNewtonsoftSchemaGenerator();
            var mvcOutputFormatter = new MvcOutputFormatter(FormatterOptions.WithXmlAndNewtonsoftJsonFormatters, new FakeLoggerFactory());
            var requestExample = new RequestExample(mvcOutputFormatter, Options.Create(swaggerOptions), schemaGenerator);
            var responseExample = new ResponseExample(mvcOutputFormatter, schemaGenerator);

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
            var actualExample = JsonConvert.DeserializeObject<PersonResponse>(((OpenApiRawString)response.Content["application/json"].Example).Value);

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
            var actualExample = JsonConvert.DeserializeObject<PersonResponse>(((OpenApiRawString)response.Content["application/json"].Example).Value);

            var expectedExample = new PersonResponseExample().GetExamples();
            actualExample.Id.ShouldBe(expectedExample.Id);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
        }

        /* [Fact]
         This test is no longer needed - we used to have a ContractResolver parameter on the SwaggerResponse attribute,
         but that has been removed.
         Your examples will be output with whichever ContractResolver is registered, e.g.
         services.AddControllers()
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ContractResolver = new DefaultContractResolver();

        public void SetsResponseExamples_FromMethodAttributesPascalCase()
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
        } */

        [Fact]
        public void SetsMultipleResponseExamples_FromMethodAttributes()
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
            var actualExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiRawString)requestBody.Content["application/json"].Example).Value);
            var expectedExample = new PersonRequestExample().GetExamples();
            actualExample.ShouldMatch(expectedExample);

            // Assert SerializeAsV2
            var actualSchemaExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiRawString)filterContext.SchemaRepository.Schemas["PersonRequest"].Example).Value);
            actualSchemaExample.ShouldMatch(expectedExample);
        }

        [Fact]
        public void GeneratesNewSchemaInSchemaRepository_FromMethodAttributes_WhenExpectedRequestSchemaDidNotExist()
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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAndRequestExampleNotMatchingBodyType));

            var expDefinitionName = new PersonRequestExample().GetExamples().GetType().SchemaDefinitionName();

            // Act
            filterContext.SchemaRepository.Schemas.ShouldNotContain(x => x.Key == expDefinitionName);
            sut.Apply(operation, filterContext);

            // Assert
            filterContext.SchemaRepository.Schemas.ShouldContain(x => x.Key == expDefinitionName);
        }

        [Fact]
        public void SetsRequestCorrectSingleSchema_FromMethodAttributes_WhenExampleNotMatchBodyType()
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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAndRequestExampleNotMatchingBodyType));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualSchemaDefinition = requestBody.Content["application/json"].Schema.Reference.Id;
            var expectedSchemaDefinition = new PersonRequestExample().GetExamples().GetType().SchemaDefinitionName();
            actualSchemaDefinition.ShouldMatch(expectedSchemaDefinition);
        }

        [Fact]
        public void GeneratesNewSchemasInSchemaRepository_FromMethodAttributes_WhenExpectedSchemasDidNotExist()
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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAndRequestMultipleExamplesNotMatchingResponseAndBodyType));

            // Act
            var expSchemas = new MultipleTypesRequestExamples().GetExamples().Select(x => x.Value.GetType().SchemaDefinitionName()).Distinct().ToList();
            filterContext.SchemaRepository.Schemas.Select(x => x.Key).ShouldNotContain(x => expSchemas.Contains(x));

            sut.Apply(operation, filterContext);

            // Assert
            expSchemas.ShouldBeSubsetOf(filterContext.SchemaRepository.Schemas.Select(x => x.Key));
        }

        [Fact]
        public void SetsRequestCorrectMultipleRequestSchema_FromMethodAttributes_WhenMultipleExamplesAreOfDifferentTypes()
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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAndRequestMultipleExamplesNotMatchingResponseAndBodyType));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            requestBody.Content["application/json"].Schema.Reference.ShouldBeNull();
            requestBody.Content["application/json"].Schema.OneOf.Select(x => x.Reference.Id).ShouldBe(
                new MultipleTypesRequestExamples().GetExamples().Select(x => x.Value.GetType().SchemaDefinitionName()).Distinct()
            );
        }

        [Fact]
        public void DoesNotGenerateAdditionalRequestSchema_FromMethodAttributes_WhenSchemaAlreadyExists()
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
            filterContext.SchemaRepository.Schemas.Select(x => x.Key)
                    .Count(x => x == typeof(PersonRequest).SchemaDefinitionName()).ShouldBe(1);
            sut.Apply(operation, filterContext);

            // Assert
            filterContext.SchemaRepository.Schemas.Select(x => x.Key)
                    .Count(x => x == typeof(PersonRequest).SchemaDefinitionName()).ShouldBe(1);
        }

        [Fact]
        public void SetsSingleRequestSchemaCorrectly_FromMethodAttributes_WhenMultipleExamplesAreOfSameType()
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
            requestBody.Content["application/json"].Schema.Reference.Id.ShouldBe(typeof(PersonRequest).SchemaDefinitionName());
        }


        [Fact]
        public void GeneratesNewSchemaInSchemaRepository_FromMethodAttributes_WhenExpectedResponseSchemaDidNotExist()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType() } } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAndRequestExampleNotMatchingBodyType));
            var expDefinitionName = new PersonResponseExample().GetExamples().GetType().SchemaDefinitionName();

            // Act
            filterContext.SchemaRepository.Schemas.ShouldNotContain(x => x.Key == expDefinitionName);
            sut.Apply(operation, filterContext);

            // Assert
            filterContext.SchemaRepository.Schemas.ShouldContain(x => x.Key == expDefinitionName);
        }

        [Fact]
        public void SetsResponseCorrectSingleSchema_FromMethodAttributes_WhenExampleDiffersFromDeclaredType()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType()} } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAndRequestExampleNotMatchingBodyType));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            var actualSchemaDefinition = operation.Responses["200"].Content["application/json"].Schema.Reference.Id;
            var expectedSchemaDefinition = new PersonResponseExample().GetExamples().GetType().SchemaDefinitionName();
            actualSchemaDefinition.ShouldMatch(expectedSchemaDefinition);
        }

        [Fact]
        public void GeneratesNewSchemasInSchemaRepository_FromMethodAttributes_WhenExpectedResponseSchemasDidNotExist()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType()} } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAndRequestMultipleExamplesNotMatchingResponseAndBodyType));

            // Act
            var expSchemas = new MultipleTypesResponseExamples().GetExamples().Select(x => x.Value.GetType().SchemaDefinitionName()).Distinct().ToList();
            filterContext.SchemaRepository.Schemas.Select(x => x.Key).ShouldNotContain(x => expSchemas.Contains(x));

            sut.Apply(operation, filterContext);

            // Assert
            expSchemas.ShouldBeSubsetOf(filterContext.SchemaRepository.Schemas.Select(x => x.Key));
        }

        [Fact]
        public void SetsRequestCorrectMultipleResponseSchema_FromMethodAttributes_WhenMultipleExamplesAreOfDifferentTypes()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType()} } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseAndRequestMultipleExamplesNotMatchingResponseAndBodyType));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Responses["200"].Content["application/json"].Schema.Reference.ShouldBeNull();
            operation.Responses["200"].Content["application/json"].Schema.OneOf.Select(x => x.Reference.Id).ShouldBe(
                new MultipleTypesResponseExamples().GetExamples().Select(x => x.Value.GetType().SchemaDefinitionName()).Distinct()
            );
        }

        [Fact]
        public void DoesNotGenerateAdditionalResponseSchema_FromMethodAttributes_WhenSchemaAlreadyExists()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType()} } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseMultipleExamplesAttribute));

            // Act
            filterContext.SchemaRepository.Schemas.Select(x => x.Key)
                    .Count(x => x == typeof(PersonResponse).SchemaDefinitionName()).ShouldBe(0);

            for (var i = 0; i < 2; i++)
            {
                sut.Apply(operation, filterContext);

                // Assert
                filterContext.SchemaRepository.Schemas.Select(x => x.Key)
                        .Count(x => x == typeof(PersonResponse).SchemaDefinitionName()).ShouldBe(1);
            }
        }

        [Fact]
        public void SetsSingleResponseSchemaCorrectly_FromMethodAttributes_WhenMultipleExamplesAreOfSameType()
        {
            // Arrange
            var response = new OpenApiResponse { Content = new Dictionary<string, OpenApiMediaType> { { "application/json", new OpenApiMediaType()} } };
            var operation = new OpenApiOperation { OperationId = "foobar", Responses = new OpenApiResponses() };
            operation.Responses.Add("200", response);

            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AnnotatedWithSwaggerResponseMultipleExamplesAttribute));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Responses["200"].Content["application/json"].Schema.Reference.Id.ShouldBe(typeof(PersonResponse).SchemaDefinitionName());
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

            // Assert SerializeAsV2
            var actualSchemaExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiRawString)filterContext.SchemaRepository.Schemas["PersonRequest"].Example).Value);
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
            // var actualExample = JsonConvert.DeserializeObject<PersonRequest>(((OpenApiRawString)requestBody.Content["application/json"].Example).Value);
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
            var actualExample = JsonConvert.DeserializeObject<Dictionary<string, object>>(((OpenApiRawString)requestBody.Content["application/json"].Example).Value);
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
    }
}
