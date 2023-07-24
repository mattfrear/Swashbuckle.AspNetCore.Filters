using Microsoft.AspNetCore.Hosting;
using Swashbuckle.AspNetCore.Filters;
#if NETCOREAPP3_1_OR_GREATER
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;
#endif

namespace WebApi.Models.Examples
{
    internal class PersonRequestDependencyInjectionExample : IExamplesProvider<PersonRequest>
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public PersonRequestDependencyInjectionExample(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public PersonRequest GetExamples()
        {
            return new PersonRequest { Title = Title.Mr, Age = 24, FirstName = $"Dave ({hostingEnvironment.EnvironmentName})", Income = null };
        }
    }
}