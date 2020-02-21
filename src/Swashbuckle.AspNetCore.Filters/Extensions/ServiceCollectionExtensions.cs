using Microsoft.Extensions.DependencyInjection;

namespace Swashbuckle.AspNetCore.Filters
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerExamplesFromAssemblyOf<T>(this IServiceCollection services)
        {
            services.AddSingleton<SerializerSettingsDuplicator>();
            services.AddSingleton<JsonFormatter>();
            services.AddSingleton<RequestExample>();
            services.AddSingleton<ResponseExample>();
            services.AddSingleton<ExamplesOperationFilter>();
            services.AddSingleton<ServiceProviderExamplesOperationFilter>();
            services.AddSingleton<MvcOutputFormatter>();

            services.Scan(scan => scan
                .FromAssemblyOf<T>()
                    .AddClasses(classes => classes.AssignableTo(typeof(IExamplesProvider<>)))
                    .AsImplementedInterfaces()
                    .AsSelf()
                    .WithSingletonLifetime()
            );

            return services;
        }
    }
}
