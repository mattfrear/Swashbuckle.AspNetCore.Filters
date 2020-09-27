using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Text.Json.Serialization;
using WebApi.Models.Examples;

namespace WebApi3._0_Swashbuckle5
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
            services.AddControllers()
                //.AddNewtonsoftJson(opt =>
                //{
                //    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
                //    // opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                //})
                .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                // .AddXmlSerializerFormatters();
                .AddXmlDataContractSerializerFormatters();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v2" });

                options.ExampleFilters();

                options.OperationFilter<AddHeaderOperationFilter>("correlationId", "Correlation Id for the request");

                options.OperationFilter<AddResponseHeadersFilter>();

                options.DescribeAllEnumsAsStrings();

                var filePath = Path.Combine(AppContext.BaseDirectory, "WebApi.xml");
                options.IncludeXmlComments(filePath);

                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();

                options.IgnoreObsoleteProperties();
            });

            services.AddSwaggerExamplesFromAssemblyOf<PersonResponseExample>();

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(
                // c => c.SerializeAsV2 = true
            );

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
