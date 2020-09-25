﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Buffers;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes
{
    internal class FormatterOptions : IOptions<MvcOptions>
    {
        private FormatterOptions(params IOutputFormatter[] formatters)
        {
            Value = new MvcOptions();
            foreach (var formatter in formatters)
                Value.OutputFormatters.Add(formatter);
        }

        public MvcOptions Value { get; }
        
        public static FormatterOptions WithXmlDataContractFormatter
            => new FormatterOptions(new XmlDataContractSerializerOutputFormatter());

        public static FormatterOptions WithNewtonsoftFormatter
            => new FormatterOptions(new JsonOutputFormatter(new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared));

        public static FormatterOptions WithoutFormatters
            => new FormatterOptions();
    }
}