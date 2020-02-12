using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class MvcOutputFormatter
    {
        private readonly OutputFormatterSelector outputFormatterSelector;

        public MvcOutputFormatter(IOptions<MvcOptions> options, ILoggerFactory loggerFactory)
        {
            if (options?.Value?.OutputFormatters?.Count > 0)
            {
                var selectorOptions = new MvcOptions
                {
                    ReturnHttpNotAcceptable = true,
                    RespectBrowserAcceptHeader = true
                };
                foreach (var formatter in options.Value.OutputFormatters)
                    selectorOptions.OutputFormatters.Add(formatter);

                this.outputFormatterSelector = new DefaultOutputFormatterSelector(
                    new OptionsWrapper<MvcOptions>(selectorOptions),
                    loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))
                );
            }
        }

        public string Serialize<T>(T value, MediaTypeHeaderValue contentType)
        {
            if (outputFormatterSelector == null)
                throw new FormatterNotFound(contentType);

            if (value == null)
                return string.Empty;

            using (var stringWriter = new StringWriter())
            {
                var outputFormatterContext = GetOutputFormatterContext(
                    stringWriter,
                    value,
                    value.GetType(),
                    contentType);

                var formatter = outputFormatterSelector.SelectFormatter(
                    outputFormatterContext,
                    new List<IOutputFormatter>(),
                    new MediaTypeCollection());

                if (formatter == null)
                    throw new FormatterNotFound(contentType);

                formatter.WriteAsync(outputFormatterContext).GetAwaiter().GetResult();
                stringWriter.FlushAsync().GetAwaiter().GetResult();
                return stringWriter.ToString();
            }
        }

        private static OutputFormatterWriteContext GetOutputFormatterContext(
            TextWriter writer,
            object outputValue,
            Type outputType,
            MediaTypeHeaderValue contentType)
        {
            return new OutputFormatterWriteContext(
                GetHttpContext(contentType),
                (stream, encoding) => writer,
                outputType,
                outputValue);
        }

        private static HttpContext GetHttpContext(MediaTypeHeaderValue contentType)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.AcceptCharset] = contentType.Charset.ToString();
            httpContext.Request.Headers[HeaderNames.Accept] = contentType.MediaType.Value;
            httpContext.Request.ContentType = contentType.MediaType.Value;

            httpContext.Response.Body = new MemoryStream();
            httpContext.RequestServices =
                new ServiceCollection()
                    .AddSingleton(Options.Create(new MvcOptions()))
                    .BuildServiceProvider();

            return httpContext;
        }
        internal class FormatterNotFound : Exception
        {
            public FormatterNotFound(MediaTypeHeaderValue contentType)
                : base($"OutputFormatter not found for '{contentType}'")
            { }
        }
    }

}