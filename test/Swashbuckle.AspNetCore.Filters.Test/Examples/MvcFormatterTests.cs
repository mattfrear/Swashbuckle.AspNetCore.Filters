﻿using Microsoft.AspNetCore.Mvc;
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
                .ShouldBe("{\"title\":3,\"age\":24,\"firstName\":\"Dave\",\"income\":null,\"children\":null,\"job\":null}");
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
        private readonly IServiceProvider serviceProvider;

        public GivenAMvcFormatterWitXmlOutputFormatter_WhenSerializingAnObjectAsXml()
        {
            serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IOptions<MvcOptions>)).Returns(Options.Create(new MvcOptions()));

            sut = new MvcOutputFormatter(FormatterOptions.WithXmlDataContractFormatter, serviceProvider, new FakeLoggerFactory());
        }

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