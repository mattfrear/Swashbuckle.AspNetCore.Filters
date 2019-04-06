using Microsoft.AspNetCore.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class PersonRequestDependencyInjectionExample : IExamplesProvider
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public PersonRequestDependencyInjectionExample(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public object GetExamples()
        {
            return new PersonRequest { Title = Title.Mr, Age = 24, FirstName = $"Dave ({hostingEnvironment.EnvironmentName})", Income = null };
        }
    }
}