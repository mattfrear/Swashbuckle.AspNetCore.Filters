using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using NSubstitute;
using Shouldly;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using System;
using System.Xml.Linq;
using Xunit;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class GivenAMvcFormatterWithNoOutputFormatters_WhenSerializingAnObject
    {
        private readonly MvcOutputFormatter sut;

        public GivenAMvcFormatterWithNoOutputFormatters_WhenSerializingAnObject()
            => sut = new MvcOutputFormatter(FormatterOptions.WithoutFormatters, null, null);

        [Fact]
        public void ThenAFormatNotFoundExceptionIsThrown()
        {
            var value = new PersonResponseExample().GetExamples();
            var contentType = MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");

            Should
                .Throw<MvcOutputFormatter.FormatterNotFoundException>(() => sut.Serialize(value, contentType))
                .Message.ShouldBe($"OutputFormatter not found for '{contentType}'");
        }

        [Fact]
        public void ThenWillFallbackToSystemTextJson()
        {
            var value = new PersonRequestExample().GetExamples();
            var contentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            sut.Serialize(value, contentType)
                .ShouldBe("{\"Title\":3,\"Age\":24,\"FirstName\":\"Dave\",\"Income\":null,\"Children\":null,\"Job\":null}");
        }

    }

    public class GivenAMvcFormatterWithOutputFormattersButNoLoggerFactory_WhenSerializingAnObject
    {
        private readonly MvcOutputFormatter sut;

        public GivenAMvcFormatterWithOutputFormattersButNoLoggerFactory_WhenSerializingAnObject()
        {
            sut = new MvcOutputFormatter(FormatterOptions.WithXmlDataContractFormatter, null, null);
        }

        [Fact]
        public void ThenAnArgumentNullExceptionIsThrown()
        {
            var value = new PersonResponseExample().GetExamples();
            var contentType = MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");

            Should
                .Throw<ArgumentNullException>(() => sut.Serialize(value, contentType))
                .ParamName.ShouldBe("loggerFactory");
        }
    }

    public class GivenAMvcFormatterWithOutputFormatters_WhenSerializingAnObjectForAContentTypeThatIsNotConfigured
    {
        private readonly MvcOutputFormatter sut;

        public GivenAMvcFormatterWithOutputFormatters_WhenSerializingAnObjectForAContentTypeThatIsNotConfigured()
            => sut = new MvcOutputFormatter(FormatterOptions.WithXmlDataContractFormatter, null, new FakeLoggerFactory());

        [Fact]
        public void ThenAFormatNotFoundExceptionIsThrown()
        {
            var value = new PersonResponseExample().GetExamples();
            var contentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            Should
                .Throw<MvcOutputFormatter.FormatterNotFoundException>(() => sut.Serialize(value, contentType))
                .Message.ShouldBe($"OutputFormatter not found for '{contentType}'");
        }
    }

    public class GivenAMvcFormatterWitXmlOutputFormatter_WhenSerializingAnObjectAsXml
    {
        private readonly MvcOutputFormatter sut;

        public GivenAMvcFormatterWitXmlOutputFormatter_WhenSerializingAnObjectAsXml()
            => sut = new MvcOutputFormatter(FormatterOptions.WithXmlDataContractFormatter, null, new FakeLoggerFactory());

        [Fact]
        public void ThenAnXmlStringIsReturned()
        {
            var value = new PersonResponseExample().GetExamples();
            var contentType = MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");

            var serializedValue = sut.Serialize(value, contentType);
            Should.NotThrow(() => XDocument.Parse(serializedValue));
        }

        [Fact]
        public void ThenValueIsSerialized()
        {
            var value = new PersonResponseExample().GetExamples();
            var contentType = MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");

            sut.Serialize(value, contentType)
                .ShouldBe(
                    "<PersonResponse xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples\">" +
                    "<Age>27</Age>" +
                    "<Id>123</Id>" +
                    "<Income i:nil=\"true\" />" +
                    "<Title>Dr</Title>" +
                    "<first>John</first>" +
                    "</PersonResponse>");
        }
    }
    public class GivenAnMvcFormatterWithAnOutputFormatter_WhenAServiceProviderIsInjected
    {
        private readonly MvcOutputFormatter sut;

        private static IServiceProvider GetServiceProvider()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(string)).Returns("somePropName");
            var mvcOptions = Substitute.For<IOptions<MvcOptions>>();
            serviceProvider.GetService(typeof(IOptions<MvcOptions>)).Returns(mvcOptions);
            return serviceProvider;
        }

        public GivenAnMvcFormatterWithAnOutputFormatter_WhenAServiceProviderIsInjected()
            => sut = new MvcOutputFormatter(
                FormatterOptions.WithFormatterAccessingRequestServices,
                GetServiceProvider(),
                new FakeLoggerFactory());

        [Fact]
        public void TheServiceProviderIsPassedToTheOutputFormatterContext()
        {
            var contentType = MediaTypeHeaderValue.Parse("text/plain; charset=utf-8");

            sut.Serialize(32, contentType)
                .ShouldBe("somePropName=32");
        }
    }
}