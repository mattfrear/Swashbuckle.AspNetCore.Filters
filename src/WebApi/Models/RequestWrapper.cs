namespace WebApi.Models
{
    public class RequestWrapper<T>
    {
        public T Body { get; set; }
    }
}