using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Buffers;
using System.Text.Json;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes
{
    internal class FormatterOptions : IOptions<MvcOptions>
    {
        private static NewtonsoftJsonOutputFormatter jsonOutputFormatter = new NewtonsoftJsonOutputFormatter(
            new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
            },
            ArrayPool<char>.Shared,
            new MvcOptions());

        private static SystemTextJsonOutputFormatter systemTextJsonFormatter = new SystemTextJsonOutputFormatter(
            new JsonSerializerOptions { WriteIndented = true });

        private FormatterOptions(params IOutputFormatter[] formatters)
        {
            Value = new MvcOptions();
            foreach (var formatter in formatters)
                Value.OutputFormatters.Add(formatter);
        }

        public MvcOptions Value { get; }
        
        public static FormatterOptions WithXmlDataContractFormatter
            => new FormatterOptions(new XmlDataContractSerializerOutputFormatter());

        public static FormatterOptions WithNewtonsoftFormatter => new FormatterOptions(jsonOutputFormatter);

        public static FormatterOptions WithSystemTextJsonFormatter = new FormatterOptions(systemTextJsonFormatter);
        public static FormatterOptions WithXmlAndNewtonsoftJsonFormatters => new FormatterOptions(new XmlSerializerOutputFormatter(), jsonOutputFormatter);
        public static FormatterOptions WithoutFormatters => new FormatterOptions();
    }
}