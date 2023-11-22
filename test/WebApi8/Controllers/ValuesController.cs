﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using WebApi.Models;
using WebApi.Models.Examples;

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [SwaggerResponse(404, type: typeof(ErrorResponse), description: "Could not find the person")]
    [SwaggerResponseExample(404, typeof(NotFoundResponseExample))]
    [SwaggerResponseHeader(new int[] { 200, 401, 403, 404 }, "CustomHeader", "string", "A custom header")]
    public class ValuesController : ControllerBase
    {
        [HttpPost]
        [Route("api/test/string/endpoint")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(string))]
        [SwaggerResponseExample(200, typeof(StringResponseExample))]
        [SwaggerRequestExample(typeof(PersonRequest), typeof(StringRequestExample))]
        public IActionResult StringEndpoint(string input)
        {
            return Ok(input);
        }

        ///// <summary>
        ///// Gets all people
        ///// </summary>
        ///// <returns>All people.</returns>
        [HttpGet]
        [Route("api/values/people")]
        [SwaggerResponse(200, type: typeof(IEnumerable<PersonResponse>), description: "Successfully found the person")]
        [SwaggerResponseExample(200, typeof(PeopleResponseExample))]
        [SwaggerResponse(500, type: null, description: "There was an unexpected error")]
        [SwaggerResponseExample(500, typeof(InternalServerResponseExample))]
        [Authorize("Customer")]
        [ProducesResponseType(401)]
        public IEnumerable<PersonResponse> GetPeople()
        {
            return new PersonResponse[]
            {
                new PersonResponse { Id = 1, FirstName = "Dave" },
                new PersonResponse { Id = 2, FirstName = "Sally" },
            };
        }

        ///// <summary>
        ///// Gets a person
        ///// </summary>
        ///// <param name="personId"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("api/values/person/{personId}")]
        [SwaggerResponse(200, type: typeof(PersonResponse), description: "Successfully found the person")]
        [SwaggerResponseExample(200, typeof(PersonResponseExample))]
        [SwaggerResponse(500, type: null, description: "There was an unexpected error")]
        [SwaggerResponseExample(500, typeof(InternalServerResponseExample))]
        [Authorize("Customer")]
        [ProducesResponseType(401)]
        public PersonResponse GetPerson(int personId)
        {
            var personResponse = new PersonResponse { Id = personId, FirstName = "Dave" };
            return personResponse;
        }

        /// <summary>
        /// Posts a person
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        /// <response code="401">You are not authorized to access this endpoint</response>
        [HttpPost]
        [Route("api/values/person")]
        [SwaggerResponse(200, type: typeof(PersonResponse), description: "Successfully found the person")]
        [SwaggerResponseExample(200, typeof(PersonResponseExample))]
        [SwaggerResponse(500, type: null, description: "There was an unexpected error")]
        [SwaggerResponseExample(500, typeof(InternalServerResponseExample))]

        [SwaggerRequestExample(typeof(PersonRequest), typeof(PersonRequestExample))]

        [SwaggerResponseHeader(StatusCodes.Status200OK, "Location", "string", "Location of the newly created resource")]
        [SwaggerResponseHeader(200, "ETag", "string", "An ETag of the resource")]

        [Authorize("Customer")]
        public PersonResponse PostPerson([FromBody] PersonRequest personRequest)
        {
            var personResponse = new PersonResponse { Id = 1, FirstName = "Dave" };
            return personResponse;
        }

        /// <summary>
        /// Posts a person. Multiple request body and response examples provided.
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        /// <response code="401">You are not authorized to access this endpoint</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/values/anyone")]
        [SwaggerResponse(200, type: typeof(PersonResponse), description: "Successfully added the person")]
        [SwaggerRequestExample(typeof(PersonRequest), typeof(PersonRequestMultipleExamples))]
        [SwaggerResponseExample(200, typeof(PersonResponseMultipleExamples))]
        public PersonResponse PostAnyone([FromBody] PersonRequest personRequest)
        {
            var personResponse = new PersonResponse { Id = 1, FirstName = "Dave", LastName = "Multi" };
            return personResponse;
        }

        /// <summary>
        /// No [SwaggerRequestExample] or [SwaggerResponseExample] attributes on this one
        /// </summary>
        /// <param name="maleRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/values/male/")]
        [ProducesResponseType(typeof(PersonResponse), 200)]
        public PersonResponse PostMale([FromBody] MaleRequest maleRequest)
        {
            var personResponse = new PersonResponse { Id = 7, FirstName = "Dave" };
            return personResponse;
        }

        [HttpPost]
        [Route("api/values/genericperson")]
        [SwaggerResponse(200, type: typeof(ResponseWrapper<PersonResponse>), description: "Successfully found the person")]
        [SwaggerResponseExample(200, typeof(WrappedPersonResponseExample))]
        [SwaggerRequestExample(typeof(RequestWrapper<PersonRequest>), typeof(WrappedPersonRequestExample))]
        [Authorize(Roles = "Customer")]
        public ResponseWrapper<PersonResponse> PostGenericPerson([FromBody] RequestWrapper<PersonRequest> personRequest)
        {
            var personResponse = new ResponseWrapper<PersonResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Body = new PersonResponse { Id = 1, FirstName = "Dave" }
            };

            return personResponse;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/values/listperson")]
        [SwaggerResponse(200, type: typeof(IEnumerable<PersonResponse>), description: "Successfully found the people")]
        [SwaggerRequestExample(typeof(IEnumerable<PeopleRequest>), typeof(ListPeopleRequestExample))]
        public IEnumerable<PersonResponse> PostPersonList([FromBody] List<PeopleRequest> peopleRequest)
        {
            var people = new[] { new PersonResponse { Id = 1, FirstName = "Sally" } };
            return people;
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
        [SwaggerRequestExample(typeof(Dictionary<string, object>), typeof(DictionaryRequestExample))]
        [Consumes("application/json")] // exclude text/csv
        [Produces("application/json")] // exclude text/csv
        public Dictionary<string, object> PostDictionary([FromBody] Dictionary<string, object> dynamicDictionary)
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
        [SwaggerRequestExample(typeof(DynamicData), typeof(DynamicDataRequestExample))]
        public DynamicData GetData([FromBody] DynamicData personRequest)
        {
            var personResponse = new DynamicData();
            personResponse.Payload.Add("Property", "val");
            return personResponse;
        }

        [HttpPost]
        [Route("api/values/differentperson")]
        [SwaggerRequestExample(typeof(PersonRequest), typeof(PersonRequestExample2))]
        [SwaggerResponse(200, type: typeof(PersonResponse), description: "Successfully found the person")]
        [SwaggerResponseExample(200, typeof(PersonResponseExample2))]
        public PersonResponse PostDifferentPerson([FromBody] PersonRequest personRequest)
        {
            var personResponse = new PersonResponse { Id = 1, FirstName = "Dave" };
            return personResponse;
        }

        [HttpPost]
        [Route("api/values/dependencyinjectionperson")]
        [SwaggerResponse(200, type: typeof(PersonResponse), description: "Successfully found the person")]
        [SwaggerRequestExample(typeof(PersonRequest), typeof(PersonRequestDependencyInjectionExample))]
#if NETCOREAPP3_0
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
#endif
        public PersonResponse PostDependencyInjectedExampleResponsePerson([FromBody] PersonRequest personRequest)
        {
            var personResponse = new PersonResponse { Id = 1, FirstName = "Dave" };
            return personResponse;
        }

        [HttpPatch]
        [Route("api/values/patchperson")]
        [SwaggerRequestExample(typeof(JsonPatchDocument<PersonRequest>), typeof(JsonPatchPersonRequestExample))] // ASP.NET Core 1.1
        [SwaggerRequestExample(typeof(Operation), typeof(JsonPatchPersonRequestExample))] // ASP.NET Core 2.0
        public PersonResponse JsonPatchPerson([FromBody] JsonPatchDocument<PersonRequest> personRequest)
        {
            var personResponse = new PersonResponse { Id = 1, FirstName = "Dave" };
            return personResponse;
        }

        [HttpPost("api/values/title")]
        public void NullableEnumTest([FromBody] Title? someEnum)
        {
        }
    }
}
