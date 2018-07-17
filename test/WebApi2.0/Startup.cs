using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using WebApi.Models.Examples;

namespace WebApi2._0
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddJsonOptions(opt => opt.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddSingleton<PersonResponseDependencyInjectionExample>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v2" });

                c.AddSwaggerExamples(services.BuildServiceProvider());

                c.OperationFilter<DescriptionOperationFilter>();
                c.OperationFilter<AuthorizationInputOperationFilter>();
                c.OperationFilter<AddFileParamTypesOperationFilter>();

                c.OperationFilter<AddHeaderOperationFilter>("correlationId", "Correlation Id for the request");

                c.OperationFilter<AddResponseHeadersFilter>();

                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                c.DescribeAllEnumsAsStrings();

                var filePath = Path.Combine(AppContext.BaseDirectory, "WebApi.xml");
                c.IncludeXmlComments(filePath);

                // c.CustomSchemaIds((type) => type.FullName);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", authBuilder => authBuilder.RequireRole("Administrator"));
                options.AddPolicy("Customer", authBuilder => authBuilder.RequireRole("Customer"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
