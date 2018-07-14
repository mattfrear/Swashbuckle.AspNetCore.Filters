using System;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes
{
    internal class FakeServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}