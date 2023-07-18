using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSubstitute;
using System.Buffers;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiContrib.Core.Formatter.Csv;

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

        private static IOutputFormatter formatterAccessingRequestServices = new FormatterAccessingRequestServices();

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
        public static FormatterOptions WithXmlAndNewtonsoftJsonAndCsvFormatters => new FormatterOptions(new XmlSerializerOutputFormatter(), jsonOutputFormatter, new CsvOutputFormatter(new CsvFormatterOptions()));
        public static FormatterOptions WithoutFormatters => new FormatterOptions();

        public static FormatterOptions WithFormatterAccessingRequestServices
            => new FormatterOptions(formatterAccessingRequestServices);

        private class FormatterAccessingRequestServices : IOutputFormatter
        {
            public bool CanWriteResult(OutputFormatterCanWriteContext context)
                => context.ObjectType == typeof(int);

            public async Task WriteAsync(OutputFormatterWriteContext context)
            {
                var propertyName = context.HttpContext.RequestServices.GetRequiredService<string>();
                var bytes = System.Text.Encoding.UTF8.GetBytes(propertyName + "=" + context.Object.ToString());
                await context.HttpContext.Response.Body.WriteAsync(bytes);
            }
        }
    }
}