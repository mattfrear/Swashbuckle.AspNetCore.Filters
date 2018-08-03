using System.ComponentModel;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    public class PersonRequest
    {
        public Title Title { get; set; }

        public int Age { get; set; }

        [Description("The first name of the person")]
        public string FirstName { get; set; }

        public decimal? Income { get; set; }

        [Description("The person's children")]
        public Child[] Children { get; set; }

        public Job Job { get; set; }
    }
}