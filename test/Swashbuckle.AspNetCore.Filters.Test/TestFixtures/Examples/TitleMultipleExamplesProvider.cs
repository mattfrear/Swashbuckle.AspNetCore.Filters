using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    public class TitleMultipleExamplesProvider : IMultipleExamplesProvider<Title>
    {
        public IEnumerable<SwaggerExample<Title>> GetExamples()
        {
            yield return new SwaggerExample<Title>
            {
                Name = "Miss Title",
                Value = Title.Miss
            };

            yield return new SwaggerExample<Title>
            {
                Name = "Mr Title",
                Value = Title.Mr
            };
        }
    }
}
