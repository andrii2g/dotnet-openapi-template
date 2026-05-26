using System.Net;
using System.Net.Http.Json;
using ProductCatalog.Api.Common;

namespace ProductCatalog.Api.Tests;

public sealed class HealthTests
{
    [Fact]
    public async Task Health_ReturnsOk()
    {
        await using var factory = new ApiFactory();
        using var client = factory.CreateApiClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.Equal("ok", body?.Status);
    }
}
