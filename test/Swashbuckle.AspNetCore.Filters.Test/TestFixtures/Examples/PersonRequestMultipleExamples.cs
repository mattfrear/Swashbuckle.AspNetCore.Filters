using System.Collections.Generic;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    internal class PersonRequestMultipleExamples : IMultipleExamplesProvider<PersonRequest>
    {
        public IEnumerable<SwaggerExample<PersonRequest>> GetExamples()
        {
            yield return SwaggerExample.Create("Dave",
                "Posts Dave",
                "Here's a description",
                new PersonRequest {FirstName = "Dave", Title = Title.Mr});
            yield return SwaggerExample.Create("Angela",
                "Let's add Angela",
                new PersonRequest {FirstName = "Angela", Title = Title.Dr});
            yield return SwaggerExample.Create("Diane",
                "Diane is also fine to post",
                new PersonRequest {FirstName = "Diane", Title = Title.Mrs, Age = 30});
            yield return SwaggerExample.Create("Michael",
                "And the last example",
                new PersonRequest {FirstName = "Michael", Income = 321.7m});
        }
    }

    internal class PersonRequestMultipleExamplesDuplicatedKeys : IMultipleExamplesProvider<PersonRequest>
    {
        public IEnumerable<SwaggerExample<PersonRequest>> GetExamples()
        {
            yield return SwaggerExample.Create("Dave",
                "Posts Dave",
                new PersonRequest {FirstName = "Dave", Title = Title.Mr});
            yield return SwaggerExample.Create("Dave",
                "Posts other Dave",
                new PersonRequest {FirstName = "Dave", Title = Title.Dr});
        }
    }

    internal class PersonRequestMultipleExamplesNull : IMultipleExamplesProvider<PersonRequest>
    {
        public IEnumerable<SwaggerExample<PersonRequest>> GetExamples()
        {
            return null;
        }
    }

    internal class PersonRequestMultipleExamplesNullExample : IMultipleExamplesProvider<PersonRequest>
    {
        public IEnumerable<SwaggerExample<PersonRequest>> GetExamples()
        {
            yield return SwaggerExample.Create("Dave",
                "Posts Dave",
                new PersonRequest { FirstName = "Dave", Title = Title.Mr });
            yield return SwaggerExample.Create<PersonRequest>("Null",
                "Posts null",
                null);
        }
    }

    internal class PersonRequestMultipleExamplesEmpty : IMultipleExamplesProvider<PersonRequest>
    {
        public IEnumerable<SwaggerExample<PersonRequest>> GetExamples()
        {
            return new List<SwaggerExample<PersonRequest>>();
        }
    }
}