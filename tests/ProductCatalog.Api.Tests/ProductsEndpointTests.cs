using System.Net;
using System.Net.Http.Json;

namespace ProductCatalog.Api.Tests;

public sealed class ProductsEndpointTests
{
    [Fact]
    public async Task ProductCrud_Works()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateApiClient();

        var createResponse = await client.PostAsJsonAsync("/api/v1/products", new
        {
            name = "Keyboard",
            price = 99.95m
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created!.Id);
        Assert.Equal("Keyboard", created.Name);

        var getResponse = await client.GetAsync($"/api/v1/products/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updateResponse = await client.PutAsJsonAsync($"/api/v1/products/{created.Id}", new
        {
            name = "Mechanical Keyboard",
            price = 149.99m
        });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/v1/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getDeletedResponse = await client.GetAsync($"/api/v1/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidBody_ReturnsBadRequest()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateApiClient();

        var response = await client.PostAsJsonAsync("/api/v1/products", new
        {
            name = "",
            price = -1m
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    private sealed record ProductDto(
        Guid Id,
        string Name,
        decimal Price,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
    );
}
