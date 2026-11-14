using FoodDelivery.Api.Contracts.Commerce;
using FoodDelivery.Api.Data;
using FoodDelivery.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Services;

public interface IPurchaseService
{
    Task<ServiceResult<IReadOnlyList<PurchaseDto>>> GetByClientAsync(int clientId, CancellationToken ct);
    Task<ServiceResult<PurchaseDto>> PurchaseAsync(int clientId, PurchaseRequest request, CancellationToken ct);
}

public sealed class PurchaseService : IPurchaseService
{
    private readonly FoodDeliveryDbContext _db;
    public PurchaseService(FoodDeliveryDbContext db) => _db = db;
    public async Task<ServiceResult<IReadOnlyList<PurchaseDto>>> GetByClientAsync(int clientId, CancellationToken ct)
    {
        if (!await _db.Clients.AnyAsync(x => x.Id == clientId, ct)) return ServiceResult<IReadOnlyList<PurchaseDto>>.Missing();
        var items = await _db.Purchases.AsNoTracking().Where(x => x.ClientId == clientId).OrderByDescending(x => x.Date).ToListAsync(ct);
        return ServiceResult<IReadOnlyList<PurchaseDto>>.Success(items.Select(PurchaseDto.From).ToArray());
    }
    public async Task<ServiceResult<PurchaseDto>> PurchaseAsync(int clientId, PurchaseRequest request, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var client = await _db.Clients.SingleOrDefaultAsync(x => x.Id == clientId, ct);
        var product = await _db.Products.SingleOrDefaultAsync(x => x.Id == request.ProductId, ct);
        if (client is null || product is null) return ServiceResult<PurchaseDto>.Missing();
        if (product.Stock < request.Quantity) return ServiceResult<PurchaseDto>.Conflicting("Insufficient product stock.");
        var total = request.Price * request.Quantity;
        if (client.Budget < total) return ServiceResult<PurchaseDto>.Conflicting("Insufficient client budget.");
        product.Stock -= request.Quantity; client.Budget -= total;
        var purchase = new Purchase { ClientId = clientId, ProductId = product.Id, Date = DateTimeOffset.UtcNow, Price = request.Price, Quantity = request.Quantity };
        _db.Purchases.Add(purchase); await _db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return ServiceResult<PurchaseDto>.Success(PurchaseDto.From(purchase));
    }
}
