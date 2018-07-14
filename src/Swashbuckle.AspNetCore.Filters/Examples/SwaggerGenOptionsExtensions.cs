using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.Filters
{
    public static class SwaggerGenOptionsExtensions
    {
        // todo, add tests for this

        public static void AddSwaggerExamples(this SwaggerGenOptions options, IServiceCollection services)
        {
            //services.AddSingleton(typeof(IRequestExample), typeof(RequestExample));
            //services.AddSingleton(typeof(IResponseExample), typeof(ResponseExample));
            //services.AddSingleton<JsonFormatter>();
            //services.AddSingleton<SerializerSettingsDuplicator>();
            //services.AddSingleton<ExamplesOperationFilter>();
            //services.AddSingleton<ExamplesProviderFactory>();

            //var serviceProvider = services.BuildServiceProvider();
            //services.AddSingleton(serviceProvider);

            //options.OperationFilter<ExamplesOperationFilter>();

            var mvcJsonOptions = new MvcJsonOptions();
            var options2 = Options.Create(mvcJsonOptions);
            var serializerSettingsDuplicator = new SerializerSettingsDuplicator(options2);

            var jsonFormatter = new JsonFormatter();

            var serviceProvider = services.BuildServiceProvider();
            var exampleProviderFactory = new ExamplesProviderFactory(serviceProvider);

            var requestExample = new RequestExample(jsonFormatter, serializerSettingsDuplicator, exampleProviderFactory);
            var responseExample = new ResponseExample(jsonFormatter, serializerSettingsDuplicator, exampleProviderFactory);

            options.OperationFilter<ExamplesOperationFilter>(requestExample, responseExample);
        }
    }
}
