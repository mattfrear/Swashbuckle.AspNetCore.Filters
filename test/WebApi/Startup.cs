using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;

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
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

                c.AddSwaggerExamples(services);

                c.OperationFilter<DescriptionOperationFilter>();
                c.OperationFilter<AuthorizationInputOperationFilter>();
                c.OperationFilter<AddFileParamTypesOperationFilter>();

                c.OperationFilter<AddHeaderOperationFilter>("correlationId", "Correlation Id for the request");

                c.OperationFilter<AddResponseHeadersFilter>();

                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                c.DescribeAllEnumsAsStrings();

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "WebApi.xml");
                c.IncludeXmlComments(filePath);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", authBuilder => authBuilder.RequireRole("Administrator"));
                options.AddPolicy("Customer", authBuilder => authBuilder.RequireRole("Customer"));
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
