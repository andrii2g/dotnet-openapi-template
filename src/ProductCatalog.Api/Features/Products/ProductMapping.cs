namespace ProductCatalog.Api.Features.Products;

public static class ProductMapping
{
    public static ProductResponse ToResponse(this Product product) =>
        new(
            product.Id,
            product.Name,
            product.Price,
            product.CreatedAt,
            product.UpdatedAt
        );
}
