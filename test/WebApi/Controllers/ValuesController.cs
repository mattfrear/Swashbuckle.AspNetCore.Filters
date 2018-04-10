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
        /// Gets dynamic data
        /// </summary>
        /// <param name="dynamicDictionary"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/values/dictionary")]
        [SwaggerResponse(200, typeof(Dictionary<string, object>), "Successfully found the data")]
        [SwaggerResponseExample(200, typeof(DictionaryResponseExample))]
        [SwaggerResponse(500, null, "There was an unexpected error")]
        [SwaggerResponseExample(500, typeof(InternalServerResponseExample))]

        [SwaggerRequestExample(typeof(Dictionary<string, object>), typeof(DictionaryRequestExample), jsonConverter: typeof(StringEnumConverter))]

        [SwaggerResponseHeader(200, "Location", "string", "Location of the newly created resource")]
        [SwaggerResponseHeader(200, "ETag", "string", "An ETag of the resource")]
        [Authorize("Customer")]
        public Dictionary<string, object> GetDictionary([FromBody]Dictionary<string, object> dynamicDictionary)
        {
            return new Dictionary<string, object>{ {"Some", 1}};
        }

        /// <summary>
        /// Gets dynamic data
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/values/data")]
        [SwaggerResponse(200, typeof(DynamicData), "Successfully found the data")]
        [SwaggerResponseExample(200, typeof(DynamicDataResponseExample))]
        [SwaggerResponse(500, null, "There was an unexpected error")]
        [SwaggerResponseExample(500, typeof(InternalServerResponseExample))]
        [SwaggerRequestExample(typeof(DynamicData), typeof(DynamicDataRequestExample), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseHeader(200, "Location", "string", "Location of the newly created resource")]
        [SwaggerResponseHeader(200, "ETag", "string", "An ETag of the resource")]
        [Authorize("Customer")]
        public DynamicData GetData([FromBody]DynamicData personRequest)
        {
            var personResponse = new DynamicData();
            personResponse.Payload.Add("Property", "val");
            return personResponse;
        }

        /// <summary>
        /// Gets a person
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/values/person")]
        [SwaggerResponse(200, typeof(PersonResponse), "Successfully found the person")]
        [SwaggerResponseExample(200, typeof(PersonResponseExample))]
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
