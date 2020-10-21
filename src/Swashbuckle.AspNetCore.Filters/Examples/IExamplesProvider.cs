namespace Swashbuckle.AspNetCore.Filters
{
    public interface IExamplesProvider<T>
    {
        T GetExamples();
    }
}
