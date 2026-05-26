using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using ProductCatalog.Api.Common;

namespace ProductCatalog.Api.OpenApi;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddProductCatalogOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(ApiConstants.OpenApiDocumentName, options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;

            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info ??= new OpenApiInfo();
                document.Info.Title = ApiConstants.ServiceName;
                document.Info.Version = ApiConstants.ApiVersion;
                document.Info.Description =
                    "Minimal API service template with contract-quality OpenAPI output.";

                document.Servers ??= [];

                if (!document.Servers.Any(server => server.Url == "/"))
                {
                    document.Servers.Add(new OpenApiServer
                    {
                        Url = "/",
                        Description = "Default relative server URL."
                    });
                }

                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, _, _) =>
            {
                operation.Responses.TryAdd(StatusCodes.Status500InternalServerError.ToString(), new OpenApiResponse
                {
                    Description = "Unexpected server error."
                });

                return Task.CompletedTask;
            });
        });

        return services;
    }
}
