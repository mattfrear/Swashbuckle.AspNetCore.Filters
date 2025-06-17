using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples
{
    [DataContract]
    public class PersonResponse
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Title Title { get; set; }

        [DataMember(Name = "first")]
        [Description("The first name of the person")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastagain")]
        [JsonProperty("last")]
        [Description("The last name of the person")]
        public string LastName { get; set; }

        [DataMember]
        [Description("His age, in years")]
        [Obsolete]
        public int Age { get; set; }

        [DataMember]
        [Description("His income, in dollars, if known. If unknown then null")]
        public decimal? Income { get; set; }

        [Description("Not a data member. This must be hidden in data contract")]
        public string InternalNeedsOnly => "For internal needs only. Should not be exposed in XML examples";
    }
}