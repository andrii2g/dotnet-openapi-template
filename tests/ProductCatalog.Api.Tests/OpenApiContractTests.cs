using System.Net;
using System.Text.Json.Nodes;

namespace ProductCatalog.Api.Tests;

public sealed class OpenApiContractTests
{
    [Fact]
    public async Task OpenApiDocument_IsAvailableInDevelopment()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateApiClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OpenApiDocument_ContainsExpectedContractBasics()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateApiClient();

        var json = await client.GetStringAsync("/openapi/v1.json");
        var document = JsonNode.Parse(json)!;

        Assert.Equal("3.1.0", document["openapi"]?.ToString());
        Assert.Equal("Product Catalog API", document["info"]?["title"]?.ToString());
        Assert.Equal("v1", document["info"]?["version"]?.ToString());

        Assert.NotNull(document["paths"]?["/health"]);
        Assert.NotNull(document["paths"]?["/api/v1/products"]);
        Assert.NotNull(document["paths"]?["/api/v1/products/{id}"]);
    }

    [Fact]
    public async Task OpenApiDocument_HasStableOperationIds()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateApiClient();

        var json = await client.GetStringAsync("/openapi/v1.json");
        var document = JsonNode.Parse(json)!;

        Assert.Equal("Products_List", document["paths"]?["/api/v1/products"]?["get"]?["operationId"]?.ToString());
        Assert.Equal("Products_Create", document["paths"]?["/api/v1/products"]?["post"]?["operationId"]?.ToString());
        Assert.Equal("Products_GetById", document["paths"]?["/api/v1/products/{id}"]?["get"]?["operationId"]?.ToString());
        Assert.Equal("Products_Update", document["paths"]?["/api/v1/products/{id}"]?["put"]?["operationId"]?.ToString());
        Assert.Equal("Products_Delete", document["paths"]?["/api/v1/products/{id}"]?["delete"]?["operationId"]?.ToString());
    }
}
