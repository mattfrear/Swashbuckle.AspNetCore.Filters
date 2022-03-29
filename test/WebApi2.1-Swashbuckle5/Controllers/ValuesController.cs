﻿using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using WebApi.Models;
using WebApi.Models.Examples;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [SwaggerResponse(404, type: typeof(ErrorResponse), description: "Could not find the person")]
    [SwaggerResponseExample(404, typeof(NotFoundResponseExample))]
    public class ValuesController : Controller
    {
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
        [SwaggerResponseHeader(new int[] { 200, 401, 403, 404 }, "CustomHeader", "string", "A custom header")]
        [Authorize("Customer")]
        public PersonResponse PostPerson([FromBody]PersonRequest personRequest)
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
        public PersonResponse PostAnyone([FromBody]PersonRequest personRequest)
        {
            var personResponse = new PersonResponse { Id = 1, FirstName = "Dave", LastName = "Multi" };
            return personResponse;
        }

        /// <summary>
        /// Posts multiple types. Multiple types request body and response examples with properly generated schema
        /// </summary>
        /// <param name="type">Type of object to handle. Person or Animal</param>
        /// <param name="requestObject"></param>
        /// <response code="400">Some request input is invalid</response>
        /// <response code="401">You are not authorized to access this endpoint</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/values/multipletypes/{type}")]
        [SwaggerResponse(200, type: typeof(object), description: "Successfully added object")]
        [SwaggerRequestExample(typeof(object), typeof(MultipleTypesRequestExamples))]
        [SwaggerResponseExample(200, typeof(MultipleTypesResponseExamples))]
        public object PostMultipleTypes([FromBody] object requestObject,
                string type)
        {
            Type objectType;
            switch (type)
            {
                case "Person": objectType = typeof(PersonResponse); break;
                case "Animal": objectType = typeof(AnimalResponse); break;
                default: return BadRequest(new ErrorResponse {ErrorCode = 400, Message = "Provided type is invalid"});
            }

            try
            {
                var deserialized = JsonConvert.DeserializeObject(requestObject.ToString(), objectType, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                });

                return deserialized;
            }
            catch
            {
                return BadRequest(new ErrorResponse {ErrorCode = 400, Message = "Request body is invalid"});
            }
        }

        /// <summary>
        /// No [SwaggerRequestExample] or [SwaggerResponseExample] attributes on this one
        /// </summary>
        /// <param name="maleRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/values/male/")]
        [ProducesResponseType(typeof(PersonResponse), 200)]
        public PersonResponse PostMale([FromBody]MaleRequest maleRequest)
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
        public ResponseWrapper<PersonResponse> PostGenericPerson([FromBody]RequestWrapper<PersonRequest> personRequest)
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
        [SwaggerRequestExample(typeof(PeopleRequest), typeof(ListPeopleRequestExample))]
        public IEnumerable<PersonResponse> PostPersonList([FromBody]List<PeopleRequest> peopleRequest)
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
        public Dictionary<string, object> PostDictionary([FromBody]Dictionary<string, object> dynamicDictionary)
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
        public DynamicData GetData([FromBody]DynamicData personRequest)
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
        public PersonResponse PostDifferentPerson([FromBody]PersonRequest personRequest)
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
        public PersonResponse PostDependencyInjectedExampleResponsePerson([FromBody]PersonRequest personRequest)
        {
            var personResponse = new PersonResponse { Id = 1, FirstName = "Dave" };
            return personResponse;
        }

        [HttpPatch]
        [Route("api/values/patchperson")]
        [SwaggerRequestExample(typeof(JsonPatchDocument<PersonRequest>), typeof(JsonPatchPersonRequestExample))] // ASP.NET Core 1.1
        [SwaggerRequestExample(typeof(Operation), typeof(JsonPatchPersonRequestExample))] // ASP.NET Core 2.0
        public PersonResponse JsonPatchPerson([FromBody]JsonPatchDocument<PersonRequest> personRequest)
        {
            var personResponse = new PersonResponse { Id = 1, FirstName = "Dave" };
            return personResponse;
        }

        [HttpPost("api/values/title")]
        public void NullableEnumTest([FromBody]Title? someEnum)
        {
        }
    }

    public enum Test
    {

    }
}
