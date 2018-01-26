using System.ComponentModel;

namespace WebApi.Models
{
    public class PersonRequest
    {
        public Title Title { get; set; }

        public int Age { get; set; }

        [Description("The first name of the person")]
        public string FirstName { get; set; }

        public decimal? Income { get; set; }
    }
}