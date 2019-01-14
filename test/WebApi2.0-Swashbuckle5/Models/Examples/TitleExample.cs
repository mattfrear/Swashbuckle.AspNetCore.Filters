using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    internal class TitleExample : IExamplesProvider<Title?>
    {
        public Title? GetExamples()
        {
            return Title.Miss;
        }
    }
}