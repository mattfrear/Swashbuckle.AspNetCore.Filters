using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Models.Examples
{
    public class PersonRequestMultipleExamples : IMultipleExamplesProvider<PersonRequest>
    {
        public IEnumerable<SwaggerExample<PersonRequest>> GetExamples()
        {
            yield return SwaggerExample.Create("Dave",
                "Posts Dave",
                "This description is rendered as _Markdown_ by *SwaggerUI*",
                new PersonRequest {FirstName = "Dave", Title = Title.Mr});
            yield return SwaggerExample.Create("Angela",
                "Let's add Angela",
                @"
## Markdown rules!

This is a realy great feature for longer descriptions.
",
                new PersonRequest {FirstName = "Angela", Title = Title.Dr});
            yield return SwaggerExample.Create("Diane",
                "Diane is also fine to post",
                @"
- enummerations
- are
- also
- supported
",
                new PersonRequest {FirstName = "Diane", Title = Title.Mrs, Age = 30});
            yield return SwaggerExample.Create("Michael",
                "And the last example",
                // not specifying a description is fine too
                new PersonRequest {FirstName = "Michael", Income = 321.7m});
        }
    }
}