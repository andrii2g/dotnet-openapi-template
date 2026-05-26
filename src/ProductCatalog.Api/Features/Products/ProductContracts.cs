using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Api.Features.Products;

public sealed class ListProductsQuery
{
    [Range(0, int.MaxValue)]
    [Description("Number of records to skip.")]
    public int Offset { get; init; } = 0;

    [Range(1, 500)]
    [Description("Maximum number of records to return.")]
    public int Limit { get; init; } = 50;
}

public sealed record CreateProductRequest(
    [property: Required]
    [property: StringLength(120, MinimumLength = 1)]
    [property: Description("Human-readable product name.")]
    string Name,

    [property: Range(typeof(decimal), "0.01", "100000.00")]
    [property: Description("Product price in the service currency.")]
    decimal Price
);

public sealed record UpdateProductRequest(
    [property: Required]
    [property: StringLength(120, MinimumLength = 1)]
    [property: Description("Human-readable product name.")]
    string Name,

    [property: Range(typeof(decimal), "0.01", "100000.00")]
    [property: Description("Product price in the service currency.")]
    decimal Price
);

public sealed record ProductResponse(
    Guid Id,
    string Name,
    decimal Price,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Offset,
    int Limit,
    int Count
);
