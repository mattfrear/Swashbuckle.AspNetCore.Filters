using Microsoft.Extensions.Logging;
using Moq;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes
{
    internal class FakeLoggerFactory : ILoggerFactory
    {
        private readonly ILoggerFactory factory;

        public FakeLoggerFactory()
        {
            var loggerMock = new Mock<ILogger>();

            var factoryMock = new Mock<ILoggerFactory>();
            factoryMock.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(loggerMock.Object);

            factory = factoryMock.Object;
        }

        public void Dispose()
            => factory.Dispose();

        public ILogger CreateLogger(string categoryName)
            => factory.CreateLogger(categoryName);

        public void AddProvider(ILoggerProvider provider)
            => factory.AddProvider(provider);
    }
}