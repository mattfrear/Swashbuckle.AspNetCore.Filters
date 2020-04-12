using Shouldly;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.Examples;
using Swashbuckle.AspNetCore.Filters.Extensions;
using System;
using Xunit;

namespace Swashbuckle.AspNetCore.Filters.Test.Examples
{
    public class TypeExtensionsTests
    {
        [Theory]
        [InlineData(typeof(PersonRequest), "PersonRequest")]
        // [InlineData(typeof(RequestWrapper<PersonRequest>), "RequestWrapper[PersonRequest]")] // Swashbuckle.AspNetCore v4
        [InlineData(typeof(RequestWrapper<PersonRequest>), "PersonRequestRequestWrapper")]      // Swashbuckle.AspNetCore v5
        [InlineData(typeof(Title?), "Title")]
        public void SchemaDefinitionName_ShouldCalculate(Type type, string expectedName)
        {
            // Arrange
            var sut = new RequestExample(null, null, null, null);

            // Act
            var result = type.SchemaDefinitionName();

            // Assert
            result.ShouldBe(expectedName);
        }
    }
}
