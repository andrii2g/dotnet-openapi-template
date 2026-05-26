using Microsoft.AspNetCore.Http.Json;
using ProductCatalog.Api.Common;
using ProductCatalog.Api.Features.Products;
using ProductCatalog.Api.OpenApi;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddApiProblemDetails();
builder.Services.AddValidation();
builder.Services.AddProductCatalogOpenApi();

builder.Services.AddSingleton<IProductStore, InMemoryProductStore>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = ApiConstants.ServiceName;
    });
}

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async httpContext =>
    {
        await TypedResults.Problem(
            title: "Unexpected server error.",
            statusCode: StatusCodes.Status500InternalServerError)
            .ExecuteAsync(httpContext);
    });
});

app.UseStatusCodePages(async statusCodeContext =>
{
    var httpContext = statusCodeContext.HttpContext;

    if (httpContext.Response.HasStarted)
    {
        return;
    }

    await TypedResults.Problem(
        title: "HTTP error.",
        statusCode: httpContext.Response.StatusCode)
        .ExecuteAsync(httpContext);
});

app.UseHttpsRedirection();

app.MapGet("/health", () => TypedResults.Ok(new HealthResponse("ok")))
    .WithName("System_GetHealth")
    .WithTags("System")
    .WithSummary("Returns service health.")
    .WithDescription("Returns a lightweight health response for probes and smoke tests.")
    .Produces<HealthResponse>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status500InternalServerError);

var api = app.MapGroup(ApiConstants.ApiBasePath);
api.MapProductsEndpoints();

app.Run();

public partial class Program;
