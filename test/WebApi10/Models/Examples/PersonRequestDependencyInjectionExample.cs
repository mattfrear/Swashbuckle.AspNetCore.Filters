using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class PersonRequestDependencyInjectionExample : IExamplesProvider<PersonRequest>
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        public PersonRequestDependencyInjectionExample(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public PersonRequest GetExamples()
        {
            return new PersonRequest { Title = Title.Mr, Age = 24, FirstName = $"Dave ({hostingEnvironment.EnvironmentName})", Income = null };
        }
    }
}