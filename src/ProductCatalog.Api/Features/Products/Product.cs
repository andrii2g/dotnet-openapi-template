namespace ProductCatalog.Api.Features.Products;

public sealed record Product(
    Guid Id,
    string Name,
    decimal Price,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
