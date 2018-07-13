using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters.Examples;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    public static class SwaggerGenOptionsExtensions
    {
        public static void AddSwaggerExamples(this SwaggerGenOptions options, IServiceCollection services)
        {
            //services.AddSingleton(typeof(IRequestExample), typeof(RequestExample));
            //services.AddSingleton(typeof(IResponseExample), typeof(ResponseExample));
            //services.AddSingleton<JsonFormatter>();
            //services.AddSingleton<SerializerSettingsDuplicator>();
            //services.AddSingleton<ExamplesOperationFilter>();

            //var serviceProvider = services.BuildServiceProvider();
            //services.AddSingleton(serviceProvider);

            //options.OperationFilter<ExamplesOperationFilter>();

            var mvcJsonOptions = new MvcJsonOptions();
            var options2 = Options.Create(mvcJsonOptions);
            var serializerSettingsDuplicator = new SerializerSettingsDuplicator(options2);

            var jsonFormatter = new JsonFormatter();

            var serviceProvider = services.BuildServiceProvider();

            var requestExample = new RequestExample(jsonFormatter, serializerSettingsDuplicator, serviceProvider);
            var responseExample = new ResponseExample(jsonFormatter, serializerSettingsDuplicator, serviceProvider);

            options.OperationFilter<ExamplesOperationFilter>(requestExample, responseExample);
        }
    }
}
