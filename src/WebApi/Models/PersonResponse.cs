using System.ComponentModel;

namespace WebApi.Models
{
    public class PersonResponse
    {
        public int Id { get; set; }

        [Description("The first name of the person")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Description("His age, in years")]
        public int Age { get; set; }

        [Description("His income, in dollars, if known. If unknown then null")]
        public decimal? Income { get; set; }
    }
}