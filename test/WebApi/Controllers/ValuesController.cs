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
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [SwaggerResponse(404, type: typeof(ErrorResponse), description: "Could not find the person")]
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
        [SwaggerResponse(200, type: typeof(PersonResponse), description: "Successfully found the person")]
        [SwaggerResponseExample(200, typeof(PersonResponseExample))]
        [SwaggerResponse(500, type: null, description: "There was an unexpected error")]
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
        [SwaggerResponse(200, type: typeof(ResponseWrapper<PersonResponse>), description: "Successfully found the person")]
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
        [SwaggerResponse(200, type: typeof(IEnumerable<PersonResponse>), description: "Successfully found the people")]
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

        /// <summary>
        /// Gets dynamic data passing a Dictionary of string, object and returns a Dictionary
        /// </summary>
        /// <param name="dynamicDictionary"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/values/dictionary")]
        [SwaggerResponse(200, type: typeof(Dictionary<string, object>), description: "Successfully found the data")]
        [SwaggerResponseExample(200, typeof(DictionaryResponseExample))]
        [SwaggerRequestExample(typeof(Dictionary<string, object>), typeof(DictionaryRequestExample), jsonConverter: typeof(StringEnumConverter))]
        public Dictionary<string, object> GetDictionary([FromBody]Dictionary<string, object> dynamicDictionary)
        {
            return new Dictionary<string, object> { { "Some", 1 } };
        }

        /// <summary>
        /// Gets dynamic data passing a DynamicData and returning a DynamicData
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/values/data")]
        [SwaggerResponse(200, type: typeof(DynamicData), description: "Successfully found the data")]
        [SwaggerResponseExample(200, typeof(DynamicDataResponseExample))]
        [SwaggerRequestExample(typeof(DynamicData), typeof(DynamicDataRequestExample), jsonConverter: typeof(StringEnumConverter))]
        public DynamicData GetData([FromBody]DynamicData personRequest)
        {
            var personResponse = new DynamicData();
            personResponse.Payload.Add("Property", "val");
            return personResponse;
        }
    }
}
