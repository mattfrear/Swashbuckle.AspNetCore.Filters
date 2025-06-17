using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Shouldly;
using Swashbuckle.AspNetCore.Filters.Test.Extensions;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using Xunit;

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
            serviceProvider.GetService(typeof(IOptions<MvcOptions>)).Returns(Options.Create(new MvcOptions()));

            var mvcOutputFormatter = new MvcOutputFormatter(FormatterOptions.WithXmlDataContractFormatter, serviceProvider, new FakeLoggerFactory());

            sut = new ServiceProviderExamplesOperationFilter(
                serviceProvider,
                new RequestExample(mvcOutputFormatter, Options.Create(new Swagger.SwaggerOptions())),
                new ResponseExample(mvcOutputFormatter, Options.Create(new Swagger.SwaggerOptions())));
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
            var parameterDescriptions = new List<ApiParameterDescription>() { new ApiParameterDescription { Type = typeof(Dictionary<string, object>), Source = BindingSource.Body } };
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
            formatedExample.Contains("<FirstName>").ShouldBeFalse();
            formatedExample.Contains("<first>").ShouldBeTrue();
        }
    }
}
