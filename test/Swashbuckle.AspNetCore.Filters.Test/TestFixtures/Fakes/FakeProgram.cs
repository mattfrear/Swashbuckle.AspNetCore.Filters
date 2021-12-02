using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/no-attributes", () => "no attributes");
app.MapGet("/allow-anonymous-attribute", [AllowAnonymous]() => "allow anonymous attribute");
app.MapGet("/authorize-attribute", [Authorize] () => "authorize attribute");
app.MapGet("/authorize-fluent", () => "authorize fluent").RequireAuthorization();
app.MapGet("/authorize-policy", () => "authorize policy").RequireAuthorization("Administrator");
app.MapGet("/authorize-policies", () => "authorize policies").RequireAuthorization("Administrator", "Customer");
app.MapGet("/authorize-role", [Authorize(Roles = "Administrator")]() => "authorize role");
app.MapGet("/authorize-roles", [Authorize(Roles = "Administrator")][Authorize(Roles="Customer")]() => "authorize roles");
app.MapGet("/authorize-roles-attribute", [Authorize(Roles = "Administrator, Customer")]() => "authorize roles");
app.MapGet("/authorize-role-policy", [Authorize(Roles = "Customer")]() => "authorize roles").RequireAuthorization("Administrator");

app.Run();

public partial class FakeProgram
{
    // Expose the FakeProgram class for use with WebApplicationFactory<FakeProgram>
}

public class FakeProgramFactory : WebApplicationFactory<FakeProgram> {}