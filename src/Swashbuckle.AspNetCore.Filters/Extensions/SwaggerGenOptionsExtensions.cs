using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Swashbuckle.AspNetCore.Filters
{
    public static class SwaggerGenOptionsExtensions
    {
        public static void AddSwaggerExamples(this SwaggerGenOptions swaggerGenOptions, IServiceProvider serviceProvider)
        {
            // I thought about registering all my types with the service collection and then using DI
            // but I'm not sure it's a good idea to pollute the user's IServiceCollection with all of my
            // types. Maybe that's OK, I don't know. 
            // To do that I need to register my dependencies BEFORE the call to AddSwaggerGen
            // Maybe newing up everything here is bad too because this could be a memory leak?

            var mvcJsonOptions = serviceProvider.GetService<IOptions<MvcJsonOptions>>();

            var serializerSettingsDuplicator = new SerializerSettingsDuplicator(mvcJsonOptions.Value.SerializerSettings);

            var jsonFormatter = new JsonFormatter();
            var exampleProviderFactory = new ExamplesProviderFactory(serviceProvider);

            var requestExample = new RequestExample(jsonFormatter, serializerSettingsDuplicator);
            var responseExample = new ResponseExample(jsonFormatter, serializerSettingsDuplicator, exampleProviderFactory);

            swaggerGenOptions.OperationFilter<ExamplesOperationFilter>(requestExample, responseExample, exampleProviderFactory);

            swaggerGenOptions.OperationFilter<ServiceProviderExamplesOperationFilter>(serviceProvider, requestExample, responseExample);
        }
    }
}
