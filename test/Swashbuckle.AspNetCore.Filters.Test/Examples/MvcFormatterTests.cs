using System;
using System.Xml.Linq;
using Microsoft.Net.Http.Headers;
using Shouldly;
using Xunit;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class GivenAMvcFormatterWithNoOutputFormatters_WhenSerializingAnObject
    {
        private readonly MvcOutputFormatter sut;

        public GivenAMvcFormatterWithNoOutputFormatters_WhenSerializingAnObject()
            => sut = new MvcOutputFormatter(FormatterOptions.WithoutFormatters, null);

        [Fact]
        public void ThenAFormatNotFoundExceptionIsThrown()
        {
            var value = new PersonResponseExample().GetExamples();
            var contentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            Should
                .Throw<MvcOutputFormatter.FormatterNotFound>(() => sut.Serialize(value, contentType))
                .Message.ShouldBe($"OutputFormatter not found for '{contentType}'");
        }
    }

    public class GivenAMvcFormatterWithOutputFormattersButNoLoggerFactory_WhenSerializingAnObject
    {
        private readonly MvcOutputFormatter sut;

        public GivenAMvcFormatterWithOutputFormattersButNoLoggerFactory_WhenSerializingAnObject()
        {
            sut = new MvcOutputFormatter(FormatterOptions.WithXmlDataContractFormatter, null);
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
            => sut = new MvcOutputFormatter(FormatterOptions.WithXmlDataContractFormatter, new FakeLoggerFactory());

        [Fact]
        public void ThenAFormatNotFoundExceptionIsThrown()
        {
            var value = new PersonResponseExample().GetExamples();
            var contentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            Should
                .Throw<MvcOutputFormatter.FormatterNotFound>(() => sut.Serialize(value, contentType))
                .Message.ShouldBe($"OutputFormatter not found for '{contentType}'");
        }
    }

    public class GivenAMvcFormatterWitXmlhOutputFormatter_WhenSerializingAnObjectAsXml
    {
        private readonly MvcOutputFormatter sut;

        public GivenAMvcFormatterWitXmlhOutputFormatter_WhenSerializingAnObjectAsXml()
            => sut = new MvcOutputFormatter(FormatterOptions.WithXmlDataContractFormatter, new FakeLoggerFactory());

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
}