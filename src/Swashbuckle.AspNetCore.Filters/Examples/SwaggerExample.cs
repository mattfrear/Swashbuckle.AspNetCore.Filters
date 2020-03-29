namespace Swashbuckle.AspNetCore.Filters
{
    /// <summary>
    /// A single example out of multiple.
    /// </summary>
    /// <typeparam name="T">Type that the example is for.</typeparam>
    public class SwaggerExample<T> : ISwaggerExample<T>
    {
        /// <summary>
        /// Name of the example.  Required.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional summary of the example.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The example value.  Required.
        /// </summary>
        public T Value { get; set; }
    }

    /// <summary>
    /// Helper class to reduce generic boilerplate
    /// </summary>
    public static class SwaggerExample
    {
        /// <summary>
        /// Create an example for a type.
        /// </summary>
        /// <param name="name">Name of the example.</param>
        /// <param name="value">Example value.</param>
        /// <typeparam name="T">Type that the example is for.</typeparam>
        /// <returns>An example for the type.</returns>
        public static SwaggerExample<T> Create<T>(string name, T value)
        {
            return new SwaggerExample<T> {Name = name, Value = value};
        }

        /// <summary>
        /// Create an example for a type.
        /// </summary>
        /// <param name="name">Name of the example.</param>
        /// <param name="summary">Summary of the example.</param>
        /// <param name="value">Example value.</param>
        /// <typeparam name="T">Type that the example is for.</typeparam>
        /// <returns>An example for the type.</returns>
        public static SwaggerExample<T> Create<T>(string name, string summary, T value)
        {
            return new SwaggerExample<T> {Name = name, Summary = summary, Value = value};
        }
    }
}
