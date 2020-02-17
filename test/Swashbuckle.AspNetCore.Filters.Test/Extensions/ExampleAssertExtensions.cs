namespace Swashbuckle.AspNetCore.Filters.Test.Extensions
{
    using Shouldly;
    using TestFixtures.Fakes.Examples;

    public static class ExampleAssertExtensions
    {
        public static void ShouldMatch(this PersonRequest actualExample, PersonRequest expectedExample)
        {
            actualExample.Title.ShouldBe(expectedExample.Title);
            actualExample.FirstName.ShouldBe(expectedExample.FirstName);
            actualExample.Age.ShouldBe(expectedExample.Age);
        }
    }
}