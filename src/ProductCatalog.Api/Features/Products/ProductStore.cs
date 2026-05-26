using System.Collections.Concurrent;

namespace ProductCatalog.Api.Features.Products;

public interface IProductStore
{
    Task<IReadOnlyList<Product>> ListAsync(int offset, int limit, CancellationToken cancellationToken);
    Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<Product?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public sealed class InMemoryProductStore : IProductStore
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public Task<IReadOnlyList<Product>> ListAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<Product> result = _products.Values
            .OrderBy(product => product.Name, StringComparer.OrdinalIgnoreCase)
            .Skip(offset)
            .Take(limit)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task<Product> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = DateTimeOffset.UtcNow;
        var product = new Product(
            Guid.NewGuid(),
            request.Name.Trim(),
            request.Price,
            now,
            now
        );

        _products[product.Id] = product;
        return Task.FromResult(product);
    }

    public Task<Product?> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_products.TryGetValue(id, out var current))
        {
            return Task.FromResult<Product?>(null);
        }

        var updated = current with
        {
            Name = request.Name.Trim(),
            Price = request.Price,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _products[id] = updated;
        return Task.FromResult<Product?>(updated);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_products.TryRemove(id, out _));
    }
}
