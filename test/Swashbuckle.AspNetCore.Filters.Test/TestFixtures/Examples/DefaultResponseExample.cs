namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes
{
    internal class DefaultResponseExample : IExamplesProvider<DefaultResponse>
    {
        public DefaultResponse GetExamples()
        {
            return new DefaultResponse { Message = "Hello!" };
        }
    }
}