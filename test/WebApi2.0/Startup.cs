using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
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
                .AddMvc(options => {
                    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                })
                .AddJsonOptions(opt => opt.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddSingleton<PersonResponseDependencyInjectionExample>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "My API", Version = "v2" });

                options.AddSwaggerExamples();

                options.OperationFilter<DescriptionOperationFilter>();

                options.OperationFilter<AddFileParamTypesOperationFilter>();

                options.OperationFilter<AddHeaderOperationFilter>("correlationId", "Correlation Id for the request");

                options.OperationFilter<AddResponseHeadersFilter>();

                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                options.DescribeAllEnumsAsStrings();

                var filePath = Path.Combine(AppContext.BaseDirectory, "WebApi.xml");
                options.IncludeXmlComments(filePath);

                // c.CustomSchemaIds((type) => type.FullName);

                options.AddSecurityDefinition("oauth2", new ApiKeyScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = "header",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

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
