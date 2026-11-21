using FoodDelivery.Api.Contracts.Commerce;
using FoodDelivery.Api.Data;
using FoodDelivery.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Services;

public interface IProductService
{
    Task<ServiceResult<ProductDto>> CookAsync(int chefId, CookProductRequest request, CancellationToken ct);
    Task<ServiceResult<IReadOnlyList<ProductDto>>> GetByChefAsync(int chefId, CancellationToken ct);
    Task<ServiceResult<object>> DeleteAsync(int chefId, int productId, CancellationToken ct);
}

public sealed class ProductService : IProductService
{
    private readonly FoodDeliveryDbContext _db;
    public ProductService(FoodDeliveryDbContext db) => _db = db;
    public async Task<ServiceResult<ProductDto>> CookAsync(int chefId, CookProductRequest request, CancellationToken ct)
    {
        if (!await _db.Chefs.AnyAsync(x => x.Id == chefId, ct)) return ServiceResult<ProductDto>.Missing();
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        var name = request.Name.Trim();
        var product = await _db.Products
            .FromSqlInterpolated($"SELECT * FROM \"Products\" WHERE \"ChefId\" = {chefId} AND \"Name\" = {name} FOR UPDATE")
            .SingleOrDefaultAsync(ct);
        if (product is null)
        {
            product = new Product
            {
                ChefId = chefId,
                Name = name,
                Type = request.Type.Trim(),
                Stock = request.Quantity,
                CookDate = DateTimeOffset.UtcNow
            };
            _db.Products.Add(product);
        }
        else
        {
            if (product.Stock > int.MaxValue - request.Quantity)
            {
                return ServiceResult<ProductDto>.Conflicting("The resulting stock exceeds the supported limit.");
            }

            product.Type = request.Type.Trim();
            product.Stock += request.Quantity;
            product.CookDate = DateTimeOffset.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return ServiceResult<ProductDto>.Success(ProductDto.From(product));
    }
    public async Task<ServiceResult<IReadOnlyList<ProductDto>>> GetByChefAsync(int chefId, CancellationToken ct)
    {
        if (!await _db.Chefs.AnyAsync(x => x.Id == chefId, ct)) return ServiceResult<IReadOnlyList<ProductDto>>.Missing();
        var products = await _db.Products.AsNoTracking().Where(x => x.ChefId == chefId).OrderBy(x => x.Name).ToListAsync(ct);
        return ServiceResult<IReadOnlyList<ProductDto>>.Success(products.Select(ProductDto.From).ToArray());
    }
    public async Task<ServiceResult<object>> DeleteAsync(int chefId, int productId, CancellationToken ct)
    {
        var product = await _db.Products.SingleOrDefaultAsync(x => x.Id == productId && x.ChefId == chefId, ct);
        if (product is null) return ServiceResult<object>.Missing();
        if (await _db.Purchases.AnyAsync(x => x.ProductId == productId, ct) || await _db.OrderProducts.AnyAsync(x => x.ProductId == productId, ct))
            return ServiceResult<object>.Conflicting("A product that has been purchased or ordered cannot be deleted.");
        _db.Products.Remove(product); await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.Success(new { Deleted = true });
    }
}
