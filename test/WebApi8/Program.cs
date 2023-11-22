using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;
using WebApi.Models.Examples;
using WebApiContrib.Core.Formatter.Csv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddCsvSerializerFormatters(new CsvFormatterOptions { CsvDelimiter = "," })
    //.AddNewtonsoftJson(opt =>
    //{
    //    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
    //    opt.SerializerSettings.ContractResolver = new ExcludeObsoletePropertiesResolver(opt.SerializerSettings.ContractResolver);
    //    // opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    //})
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddXmlSerializerFormatters();
    // .AddXmlDataContractSerializerFormatters(); builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v2" });

    options.ExampleFilters();

    options.OperationFilter<AddHeaderOperationFilter>("correlationId", "Correlation Id for the request");

    options.OperationFilter<AddResponseHeadersFilter>();

    var filePath = Path.Combine(AppContext.BaseDirectory, "WebApi8.xml");
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
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<PersonResponseExample>();

builder.Services.AddAuthorization(options =>
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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
