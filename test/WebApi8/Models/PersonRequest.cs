using System.ComponentModel;
using System.Text.Json.Serialization;

namespace WebApi.Models
{
    public class PersonRequest
    {
        public Title Title { get; set; }

        /// <summary>
        /// The person's Age, in years
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// The first name of the person
        /// </summary>
        [JsonPropertyName("first")]
        public string FirstName { get; set; }

        public decimal? Income { get; set; }
    }
}