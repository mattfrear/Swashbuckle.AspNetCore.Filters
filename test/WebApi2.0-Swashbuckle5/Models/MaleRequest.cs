using System;
using System.ComponentModel;

namespace WebApi.Models
{
    public class MaleRequest
    {
        public Title Title { get; set; }

        public int Age { get; set; }

        [Description("The first name of the person")]
        public string FirstName { get; set; }

        [Obsolete]
        public decimal? Income { get; set; }
    }
}