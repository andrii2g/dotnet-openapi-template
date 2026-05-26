using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatalog.Api.Features.Products;

public static class ProductsEndpoints
{
    public static RouteGroupBuilder MapProductsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/products")
            .WithTags("Products");

        group.MapGet(string.Empty, ListProducts)
            .WithName("Products_List")
            .WithSummary("List products.")
            .WithDescription("Returns a paginated list of products ordered by name.")
            .Produces<PagedResponse<ProductResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", GetProductById)
            .WithName("Products_GetById")
            .WithSummary("Get product by id.")
            .WithDescription("Returns one product by its unique identifier, or 404 when it does not exist.")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost(string.Empty, CreateProduct)
            .WithName("Products_Create")
            .WithSummary("Create product.")
            .WithDescription("Creates a product and returns the created resource.")
            .Accepts<CreateProductRequest>("application/json")
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:guid}", UpdateProduct)
            .WithName("Products_Update")
            .WithSummary("Update product.")
            .WithDescription("Replaces the editable fields of an existing product.")
            .Accepts<UpdateProductRequest>("application/json")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:guid}", DeleteProduct)
            .WithName("Products_Delete")
            .WithSummary("Delete product.")
            .WithDescription("Deletes a product by id when it exists.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return api;
    }

    private static async Task<Ok<PagedResponse<ProductResponse>>> ListProducts(
        [AsParameters] ListProductsQuery query,
        IProductStore store,
        CancellationToken cancellationToken)
    {
        var products = await store.ListAsync(query.Offset, query.Limit, cancellationToken);

        var response = new PagedResponse<ProductResponse>(
            products.Select(product => product.ToResponse()).ToList(),
            query.Offset,
            query.Limit,
            products.Count
        );

        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ProductResponse>, NotFound>> GetProductById(
        Guid id,
        IProductStore store,
        CancellationToken cancellationToken)
    {
        var product = await store.GetAsync(id, cancellationToken);

        return product is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(product.ToResponse());
    }

    private static async Task<Created<ProductResponse>> CreateProduct(
        CreateProductRequest request,
        IProductStore store,
        CancellationToken cancellationToken)
    {
        var product = await store.CreateAsync(request, cancellationToken);
        var response = product.ToResponse();

        return TypedResults.Created($"/api/v1/products/{response.Id}", response);
    }

    private static async Task<Results<Ok<ProductResponse>, NotFound>> UpdateProduct(
        Guid id,
        UpdateProductRequest request,
        IProductStore store,
        CancellationToken cancellationToken)
    {
        var product = await store.UpdateAsync(id, request, cancellationToken);

        return product is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(product.ToResponse());
    }

    private static async Task<Results<NoContent, NotFound>> DeleteProduct(
        Guid id,
        IProductStore store,
        CancellationToken cancellationToken)
    {
        var deleted = await store.DeleteAsync(id, cancellationToken);

        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}
