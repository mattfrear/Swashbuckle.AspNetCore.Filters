namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    internal class TitleExample : IExamplesProvider<Title?>
    {
        public Title? GetExamples()
        {
            return Title.Miss;
        }
    }
}