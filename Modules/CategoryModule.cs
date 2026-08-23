using Carter;

namespace TOTPDemo.WebAPI.Modules;

public sealed class CategoryModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder group)
    {
        var app = group.MapGroup("/categories").RequireAuthorization();

        app.MapGet(string.Empty, () =>
        {
            var categoryNames = new List<string>() { "Test 1", "Test 2", "Test 3" };
            return Results.Ok(categoryNames);
        });
    }
}
