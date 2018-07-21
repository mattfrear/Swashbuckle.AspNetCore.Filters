namespace Swashbuckle.AspNetCore.Filters
{
    public interface IExamplesProvider
    {
        object GetExamples();
    }

    public interface IAutoExamplesProvider<T> where T : class
    {
        T GetExamples();
    }
}
