using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using WebApi.Models;
using WebApi.Models.Examples;

namespace WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services
                .AddMvc()
                .AddJsonOptions(opt => opt.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

                options.ExampleFilters();

                options.OperationFilter<DescriptionOperationFilter>();

                options.OperationFilter<AddFileParamTypesOperationFilter>();

                options.OperationFilter<AddHeaderOperationFilter>("correlationId", "Correlation Id for the request", false);

                options.OperationFilter<AddResponseHeadersFilter>();

                options.DescribeAllEnumsAsStrings();

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "WebApi.xml");
                options.IncludeXmlComments(filePath);

                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                // c.CustomSchemaIds((type) => type.FullName);

                options.AddSecurityDefinition("oauth2", new ApiKeyScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = "header",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            })
            .AddSwaggerExamplesFromAssemblyOf<PersonResponseExample>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", authBuilder =>
                {
                    authBuilder.AddAuthenticationSchemes("bearer");
                    authBuilder.RequireRole("Administrator");
                });
                options.AddPolicy("Customer", authBuilder =>
                {
                    authBuilder.AddAuthenticationSchemes("Bearer");
                    authBuilder.RequireRole("Customer");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
