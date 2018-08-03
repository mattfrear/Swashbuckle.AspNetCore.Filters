using System.ComponentModel;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    public class Job
    {
        [Description("The name of the job")]
        public string Name { get; set; }
    }
}