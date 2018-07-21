using Xunit;
using Swashbuckle.AspNetCore.Swagger;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using static Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.FakeControllers;

namespace Swashbuckle.AspNetCore.Filters.Test
{
    public class AppendAuthorizeToSummaryOperationFilterTests : BaseOperationFilterTests
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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.Authorize));

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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AuthorizeAdministratorPolicy));

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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AuthorizeMultiplePolicies));

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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AuthorizeAdministratorRole));

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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AuthorizeMultipleRoles));

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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AuthorizeMultipleRolesInOneAttribute));

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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AuthorizePolicyAndRole));

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
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.None));

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
            var filterContext = FilterContextFor(typeof(AuthController), nameof(FakeActions.None));

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
            var filterContext = FilterContextFor(typeof(AuthController), nameof(AuthController.AllowAnonymous));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary");
        }


        [Fact]
        public void Apply_DoesNotAppendWhenControllerHasAllowAnonymous()
        {
            // Arrange
            var operation = new Operation { Summary = "Test summary" };
            var filterContext = FilterContextFor(typeof(AllowAnonymousController), nameof(AllowAnonymousController.Customer));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Summary.ShouldBe("Test summary");
        }
    }
}
