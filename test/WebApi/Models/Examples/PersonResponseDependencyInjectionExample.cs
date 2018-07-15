using Microsoft.AspNetCore.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class PersonResponseDependencyInjectionExample : IExamplesProvider
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public PersonResponseDependencyInjectionExample(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public object GetExamples()
        {
            return new PersonRequest { Title = Title.Mr, Age = 24, FirstName = $"Dave ({hostingEnvironment.EnvironmentName})", Income = null };
        }
    }
}