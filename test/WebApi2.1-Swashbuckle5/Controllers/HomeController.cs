using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace WebApi.Controllers
{
    [SwaggerResponse(200)]
    [SwaggerResponse(400, Type = typeof(RemoteServiceErrorInfo))]
    [SwaggerResponseExample(400, typeof(ClentErrorExamples))]
    [SwaggerResponse(500, Type = typeof(RemoteServiceErrorInfo))]
    [SwaggerResponseExample(500, typeof(ServerErrorExamples))]
    public class HomeController : ControllerBase
    {
        [HttpGet("home/index")]
        public IActionResult Index()
        {
            return NoContent();
        }
    }

    public class ServerErrorExamples : IExamplesProvider<RemoteServiceErrorInfo>
    {
        public RemoteServiceErrorInfo GetExamples()
        {
            return new RemoteServiceErrorInfo
            {
                Code = "ServerErrorExamples"
            };
        }
    }

    public class RemoteServiceErrorInfo
    {
        public string Code { get; internal set; }
    }

    public class ClentErrorExamples : IExamplesProvider<RemoteServiceErrorInfo>
    {
        public RemoteServiceErrorInfo GetExamples()
        {
            return new RemoteServiceErrorInfo
            {
                Code = "ClentErrorExamples"
            };
        }
    }
}
