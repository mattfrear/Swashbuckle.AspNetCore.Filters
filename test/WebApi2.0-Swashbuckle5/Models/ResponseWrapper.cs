using System.Net;

namespace WebApi.Models
{
    public class ResponseWrapper<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T Body { get; set; }
    }
}