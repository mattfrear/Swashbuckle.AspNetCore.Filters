# Swashbuckle.AspNetCore.Examples
Adds the [SwaggerRequestExample] and [SwaggerResponseExample] attributes to Swashbuckle.AspNetCore

Example request: https://mattfrear.com/2016/01/25/generating-swagger-example-requests-with-swashbuckle/

Example response: https://mattfrear.com/2015/04/21/generating-swagger-example-responses-with-swashbuckle/

## Examples with Dependency injection

If for some reason you need to have examples with DI (for example read them from database) you may:

Provide required dependencies in example provider classes:

```
internal class PersonRequestExample : IExamplesProvider
{
	private readonly IHostingEnvironment _env;

	public PersonRequestExample(IHostingEnvironment env)
	{
		_env = env;
	}
	public object GetExamples()
	{
		return new PersonRequest { Age = 24, FirstName = _env.IsDevelopment() ? "Development" : "Production", Income = null };
	}
}
```

Register your example provider as a service in Startup:

```
services.AddTransient<PersonRequestExample>();
```

Pass service collection to ExamplesOperationFilter as a parameter:

```
services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

	c.OperationFilter<ExamplesOperationFilter>(services.BuildServiceProvider());
	c.OperationFilter<DescriptionOperationFilter>();
});
```
