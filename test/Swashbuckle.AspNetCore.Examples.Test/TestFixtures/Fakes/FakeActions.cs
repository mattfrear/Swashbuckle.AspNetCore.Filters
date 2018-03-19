using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Examples;

namespace Swashbuckle.AspNetCore.SwaggerGen.Test
{
    public class FakeActions
    {
        [SwaggerResponse(200, typeof(PersonResponse))]
        [SwaggerResponseExample(200, typeof(PersonResponseExample))]
        public IActionResult AnnotatedWithSwaggerResponseExampleAttributes()
        {
            throw new NotImplementedException();
        }

        [SwaggerRequestExample(typeof(PersonRequest), typeof(PersonRequestExample), jsonConverter: typeof(StringEnumConverter))]
        public IActionResult AnnotatedWithSwaggerRequestExampleAttributes(PersonRequest personRequest)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        public IActionResult Authorize()
        {
            throw new NotImplementedException();
        }

        [Authorize("Administrator")]
        public IActionResult AuthorizeAdministratorPolicy()
        {
            throw new NotImplementedException();
        }

        [Authorize("Administrator")]
        [Authorize("Customer")]
        public IActionResult AuthorizeMultiplePolicies()
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult AuthorizeAdministratorRole()
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Customer")]
        public IActionResult AuthorizeMultipleRoles()
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "Administrator, Customer")]
        public IActionResult AuthorizeMultipleRolesInOneAttribute()
        {
            throw new NotImplementedException();
        }

        [Authorize(Policy = "Administrator")]
        [Authorize(Roles = "Customer")]
        public IActionResult AuthorizePolicyAndRole()
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        public IActionResult AllowAnonymous()
        {
            throw new NotImplementedException();
        }

        public IActionResult None()
        {
            throw new NotImplementedException();
        }
    }
}