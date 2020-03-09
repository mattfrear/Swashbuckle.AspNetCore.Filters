namespace Swashbuckle.AspNetCore.Filters
{
    internal interface IJsonFormatter
    {
        string FormatJson(object examples);
    }
}