using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class ControllerWithNoAttributes : ControllerBase
    {
        [HttpGet("apiv2/values/person/{personId}")]
        public PersonResponse GetPerson(int personId)
        {
            var personResponse = new PersonResponse { Id = personId, FirstName = "Dave" };
            return personResponse;
        }
    }
}
