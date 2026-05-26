# dotnet-openapi-template

ASP.NET Core Minimal API service template targeting .NET 10 with build-time OpenAPI generation, ProblemDetails, built-in validation, Scalar, and integration tests.

## Planned shape

- `ProductCatalog.slnx`
- `src/ProductCatalog.Api`
- `tests/ProductCatalog.Api.Tests`
- `openapi/product-catalog.json` generated during build
- OpenAPI contract quality enforced by .NET integration tests

## Local build

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```
