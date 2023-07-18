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
using System.Linq;
using System.Text;

namespace Swashbuckle.AspNetCore.Filters
{
    internal class MvcOutputFormatter
    {
        private readonly object initializeLock = new object();
        private bool initializedOutputFormatterSelector;

        private readonly IOptions<MvcOptions> options;
        private readonly IServiceProvider requestServices;
        private readonly ILoggerFactory loggerFactory;

        private OutputFormatterSelector outputFormatterSelector;

        private OutputFormatterSelector OutputFormatterSelector
        {
            get
            {
                lock (initializeLock)
                {
                    if (!initializedOutputFormatterSelector)
                    {
                        var selectorOptions = GetSelectorOptions(options);
                        if (selectorOptions != null)
                        {
                            outputFormatterSelector = new DefaultOutputFormatterSelector(
                                selectorOptions,
                                loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory)));
                        }
                        initializedOutputFormatterSelector = true;
                    }
                }

                return outputFormatterSelector;
            }
        }

        public MvcOutputFormatter(IOptions<MvcOptions> options, ILoggerFactory loggerFactory)
            : this(options,
                  GetDefaultRequestServices(),
                  loggerFactory)
        {
        }

        public MvcOutputFormatter(IOptions<MvcOptions> options, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            this.initializedOutputFormatterSelector = false;
            this.options = options;
            this.requestServices = serviceProvider;
            this.loggerFactory = loggerFactory;
        }

        public string Serialize<T>(T value, MediaTypeHeaderValue contentType)
        {
            if (OutputFormatterSelector == null)
            {
                return SerializeWithoutMvc(value, contentType);
            }

            if (value == null)
            {
                return string.Empty;
            }

            using (var stringWriter = new StringWriter())
            {
                var outputFormatterContext = GetOutputFormatterContext(
                    stringWriter,
                    value,
                    value.GetType(),
                    contentType,
                    requestServices);

                var formatter = OutputFormatterSelector.SelectFormatter(
                    outputFormatterContext,
                    new List<IOutputFormatter>(),
                    new MediaTypeCollection());

                if (formatter == null)
                {
                    throw new FormatterNotFoundException(contentType);
                }

                formatter.WriteAsync(outputFormatterContext).GetAwaiter().GetResult();
                stringWriter.FlushAsync().GetAwaiter().GetResult();
                var result = stringWriter.ToString();
                if (string.IsNullOrEmpty(result))
                {
                    // workaround for SystemTextJsonOutputFormatter
                    var ms = (MemoryStream)outputFormatterContext.HttpContext.Response.Body;
                    result = Encoding.UTF8.GetString(ms.ToArray());
                }

                return result;
            }
        }

        private string SerializeWithoutMvc<T>(T value, MediaTypeHeaderValue contentType)
        {
            if (contentType.MediaType.Value == "application/json")
            {
                return System.Text.Json.JsonSerializer.Serialize(value);
            }

            throw new FormatterNotFoundException(contentType);
        }

        private static IOptions<MvcOptions> GetSelectorOptions(IOptions<MvcOptions> options)
        {
            if (options?.Value?.OutputFormatters == null || !options.Value.OutputFormatters.Any())
            {
                return null;
            }

            var selectorOptions = new MvcOptions
            {
                ReturnHttpNotAcceptable = true,
                RespectBrowserAcceptHeader = true
            };

            foreach (var formatter in options.Value.OutputFormatters)
            {
                selectorOptions.OutputFormatters.Add(formatter);
            }

            return new OptionsWrapper<MvcOptions>(selectorOptions);
        }

        private static OutputFormatterWriteContext GetOutputFormatterContext(
            TextWriter writer,
            object outputValue,
            Type outputType,
            MediaTypeHeaderValue contentType,
            IServiceProvider requestServices)
        {
            return new OutputFormatterWriteContext(
                GetHttpContext(contentType, requestServices),
                (stream, encoding) => writer,
                outputType,
                outputValue);
        }

        private static HttpContext GetHttpContext(
            MediaTypeHeaderValue contentType,
            IServiceProvider requestServices)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.AcceptCharset] = contentType.Charset.ToString();
            httpContext.Request.Headers[HeaderNames.Accept] = contentType.MediaType.Value;
            httpContext.Request.ContentType = contentType.MediaType.Value;

            httpContext.Response.Body = new MemoryStream();
            httpContext.RequestServices = requestServices;

            return httpContext;
        }

        private static IServiceProvider GetDefaultRequestServices()
            => new ServiceCollection()
                    .AddSingleton(Options.Create(new MvcOptions()))
                    .BuildServiceProvider();

        internal class FormatterNotFoundException : Exception
        {
            public FormatterNotFoundException(MediaTypeHeaderValue contentType)
                : base($"OutputFormatter not found for '{contentType}'")
            { }
        }
    }
}