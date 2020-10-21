using System.ComponentModel;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    public class Child
    {
        [Description("The child's full name")]
        public string Name { get; set; }
    }
}
