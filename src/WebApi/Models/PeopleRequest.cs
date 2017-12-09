namespace WebApi.Models
{
    public class PeopleRequest
    {
        public Title Title { get; set; }

        public int Age { get; set; }

        public string FirstName { get; set; }

        public decimal? Income { get; set; }
    }
}