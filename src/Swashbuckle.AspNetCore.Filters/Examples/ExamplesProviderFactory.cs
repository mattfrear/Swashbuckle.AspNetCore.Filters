using System;

namespace Swashbuckle.AspNetCore.Filters
{
    public class ExamplesProviderFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ExamplesProviderFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IExamplesProvider Create(Type examplesProviderType)
        {
            return serviceProvider == null
                ? (IExamplesProvider)Activator.CreateInstance(examplesProviderType)
                : (IExamplesProvider)serviceProvider.GetService(examplesProviderType)
                  ?? (IExamplesProvider)Activator.CreateInstance(examplesProviderType);
        }
    }
}
