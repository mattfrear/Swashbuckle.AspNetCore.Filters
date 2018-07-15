namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    internal class PersonResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PersonResponse { Id = 123, Title = Title.Dr, FirstName = "John", LastName = "Doe", Age = 27, Income = null };
        }
    }
}