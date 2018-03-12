using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Models;
using WebApi.Models.Examples;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [SwaggerResponse(404, typeof(ErrorResponse), "Could not find the person")]
    [SwaggerResponseExample(404, typeof(NotFoundResponseExample))]
    public class ValuesController : Controller
    {
        /// <summary>
        /// Gets a person
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/values/person")]

        [SwaggerResponse(200, typeof(PersonResponse), "Successfully found the person")]
        [SwaggerResponseExample(200, typeof(PersonResponseExample))]
        // [SwaggerResponseExample(200, typeof(PersonResponseExample), jsonConverter: typeof(StringEnumConverter))]

        // [SwaggerResponse(404, typeof(ErrorResponse), "Could not find the person")]
        // [SwaggerResponseExample(404, typeof(NotFoundResponseExample))]

        [SwaggerResponse(500, null, "There was an unexpected error")]
        [SwaggerResponseExample(500, typeof(InternalServerResponseExample))]

        [SwaggerRequestExample(typeof(PersonRequest), typeof(PersonRequestExample), jsonConverter: typeof(StringEnumConverter))]

        [SwaggerResponseHeader(200, "Location", "string", "Location of the newly created resource")]
        [SwaggerResponseHeader(200, "ETag", "string", "An ETag of the resource")]
        [Authorize("Customer")]
        public PersonResponse GetPerson([FromBody]PersonRequest personRequest)
        {
            var personResponse = new PersonResponse { Id = 1, FirstName = "Dave" };
            return personResponse;
        }

        [HttpPost]
        [Route("api/values/genericperson")]
        [SwaggerResponse(200, typeof(ResponseWrapper<PersonResponse>), "Successfully found the person")]
        [SwaggerResponseExample(200, typeof(WrappedPersonResponseExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerRequestExample(typeof(RequestWrapper<PersonRequest>), typeof(WrappedPersonRequestExample), jsonConverter: typeof(StringEnumConverter))]
        [Authorize(Roles = "Customer")]
        public ResponseWrapper<PersonResponse> GetGenericPerson([FromBody]RequestWrapper<PersonRequest> personRequest)
        {
            var personResponse = new ResponseWrapper<PersonResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Body = new PersonResponse { Id = 1, FirstName = "Dave" }
            };

            return personResponse;
        }

        [HttpPost]
        [Route("api/values/listperson")]
        [SwaggerResponse(200, typeof(IEnumerable<PersonResponse>), "Successfully found the people")]
        [SwaggerRequestExample(typeof(PeopleRequest), typeof(ListPeopleRequestExample), jsonConverter: typeof(StringEnumConverter))]
        public IEnumerable<PersonResponse> GetPersonList([FromBody]List<PeopleRequest> peopleRequest)
        {
            var people = new[] { new PersonResponse { Id = 1, FirstName = "Sally" } };
            return people;
        }

        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [Authorize("Administrator")]
        [Authorize("Customer")]
        [AddSwaggerFileUploadButton]
        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            return Ok();
        }
    }
}
