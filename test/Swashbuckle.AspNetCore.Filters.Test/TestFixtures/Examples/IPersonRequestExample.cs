namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    internal class IPersonRequestAutoExample : IExamplesProvider<IPersonRequest>
    {
        public IPersonRequest GetExamples()
        {
            return new PersonRequest { Title = Title.Mr, Age = 24, FirstName = "Dave Auto", Income = null };
        }
    }
}