namespace Swashbuckle.AspNetCore.Filters
{
    public interface IExamplesProvider
    {
        object GetExamples();
    }

    public interface IExamplesProvider<T> where T : class
    {
        T GetExamples();
    }
}
