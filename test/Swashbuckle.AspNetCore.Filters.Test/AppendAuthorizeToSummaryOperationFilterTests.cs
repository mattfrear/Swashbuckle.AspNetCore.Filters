using System.Linq;
using Newtonsoft.Json;
using Xunit;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.Examples;
using Shouldly;
using System.Reflection;
using System;

namespace Swashbuckle.AspNetCore.SwaggerGen.Test
{
    public class AppendAuthorizeToSummaryOperationFilterTests
    {
        private readonly IOperationFilter sut;

        public AppendAuthorizeToSummaryOperationFilterTests()
        {
            sut = new AppendAuthorizeToSummaryOperationFilter();
        }

        [Fact]
        public void Apply_AppendsAuth()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary" };
            var filterContext = FilterContextFor(nameof(FakeActions.Authorize));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary (Auth)");
        }

        [Fact]
        public void Apply_AppendsPolicy()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary" };
            var filterContext = FilterContextFor(nameof(FakeActions.AuthorizeAdministratorPolicy));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary (Auth policies: Administrator)");
        }

        [Fact]
        public void Apply_AppendsMultiplePolicies()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary" };
            var filterContext = FilterContextFor(nameof(FakeActions.AuthorizeMultiplePolicies));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary (Auth policies: Administrator, Customer)");
        }

        [Fact]
        public void Apply_AppendsRole()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary" };
            var filterContext = FilterContextFor(nameof(FakeActions.AuthorizeAdministratorRole));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary (Auth roles: Administrator)");
        }

        [Fact]
        public void Apply_AppendsMultipleRoles()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary" };
            var filterContext = FilterContextFor(nameof(FakeActions.AuthorizeMultipleRoles));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary (Auth roles: Administrator, Customer)");
        }

        [Fact]
        public void Apply_AppendsMultipleRolesInOneAttribute()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary" };
            var filterContext = FilterContextFor(nameof(FakeActions.AuthorizeMultipleRolesInOneAttribute));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary (Auth roles: Administrator, Customer)");
        }

        [Fact]
        public void Apply_AppendsPolicyAndRole()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary" };
            var filterContext = FilterContextFor(nameof(FakeActions.AuthorizePolicyAndRole));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary (Auth policies: Administrator; roles: Customer)");
        }

        [Fact]
        public void Apply_WorksWhenNoAuthorize()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary" };
            var filterContext = FilterContextFor(nameof(FakeActions.None));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary");
        }

        [Fact]
        public void Apply_AppendsWhenAuthIsOnController()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary" };
            var filterContext = FilterContextFor(nameof(FakeActions.None), nameof(FakeControllers.AuthController), typeof(FakeControllers.AuthController));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary (Auth)");
        }

        [Fact]
        public void Apply_DoesNotAppendWhenMethodHasAllowAnonymous()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary" };
            var filterContext = FilterContextFor(nameof(FakeControllers.AuthController.AllowAnonymous), nameof(FakeControllers.AuthController), typeof(FakeControllers.AuthController));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary");
        }

        private OperationFilterContext FilterContextFor(
            string actionFixtureName,
            string controllerFixtureName = "NotAnnotated",
            Type controllerType = null)
        {
            var fakeProvider = new FakeApiDescriptionGroupCollectionProvider();
            var apiDescription = fakeProvider
                .Add("GET", "collection", actionFixtureName, controllerFixtureName)
                .ApiDescriptionGroups.Items.First()
                .Items.First();

            var mi = (controllerType ?? typeof(FakeActions)).GetMethod(actionFixtureName);

            return new OperationFilterContext(
                apiDescription,
                new SchemaRegistry(new JsonSerializerSettings()),
                mi);
        }
    }
}
