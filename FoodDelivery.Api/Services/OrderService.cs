using FoodDelivery.Api.Contracts.Commerce;
using FoodDelivery.Api.Data;
using FoodDelivery.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Services;

public interface IOrderService
{
    Task<ServiceResult<OrderDto>> CreateAsync(int clientId, CreateOrderRequest request, CancellationToken ct);
    Task<ServiceResult<IReadOnlyList<OrderDto>>> GetByChefAsync(int chefId, CancellationToken ct);
    Task<ServiceResult<OrderDto>> LaunchAsync(int chefId, int orderId, CancellationToken ct);
}

public sealed class OrderService : IOrderService
{
    private readonly FoodDeliveryDbContext _db;
    public OrderService(FoodDeliveryDbContext db) => _db = db;
    public async Task<ServiceResult<OrderDto>> CreateAsync(int clientId, CreateOrderRequest request, CancellationToken ct)
    {
        if (request.Products.Count == 0 || request.Products.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
            return ServiceResult<OrderDto>.Invalid("An order requires unique product IDs.");
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        var client = await _db.Clients.SingleOrDefaultAsync(x => x.Id == clientId, ct);
        if (client is null) return ServiceResult<OrderDto>.Missing();
        var ids = request.Products.Select(x => x.ProductId).ToArray();
        var products = await _db.Products
            .FromSqlInterpolated($"SELECT * FROM \"Products\" WHERE \"Id\" = ANY ({ids}) ORDER BY \"Id\" FOR UPDATE")
            .ToListAsync(ct);
        if (products.Count != ids.Length) return ServiceResult<OrderDto>.Missing();
        if (products.Select(x => x.ChefId).Distinct().Count() != 1)
            return ServiceResult<OrderDto>.Invalid("All products in an order must be managed by the same chef.");
        var productsById = products.ToDictionary(x => x.Id);
        foreach (var line in request.Products)
        {
            var product = productsById[line.ProductId];
            if (product.Stock < line.Quantity) return ServiceResult<OrderDto>.Conflicting($"Insufficient stock for product {product.Id}.");
        }
        var order = new FoodOrder { ClientId = clientId, ChefId = products[0].ChefId, Date = DateTimeOffset.UtcNow, Time = request.Time };
        foreach (var line in request.Products)
        {
            var product = productsById[line.ProductId];
            product.Stock -= line.Quantity;
            order.Products.Add(new OrderProduct { ProductId = product.Id, Quantity = line.Quantity });
        }
        _db.Orders.Add(order); await _db.SaveChangesAsync(ct);
        await _db.Entry(order).Collection(x => x.Products).Query().Include(x => x.Product).LoadAsync(ct);
        await transaction.CommitAsync(ct);
        return ServiceResult<OrderDto>.Success(OrderDto.From(order));
    }
    public async Task<ServiceResult<IReadOnlyList<OrderDto>>> GetByChefAsync(int chefId, CancellationToken ct)
    {
        if (!await _db.Chefs.AnyAsync(x => x.Id == chefId, ct)) return ServiceResult<IReadOnlyList<OrderDto>>.Missing();
        var orders = await _db.Orders.AsNoTracking().Include(x => x.Products).ThenInclude(x => x.Product)
            .Where(x => x.ChefId == chefId).OrderByDescending(x => x.Date).ToListAsync(ct);
        return ServiceResult<IReadOnlyList<OrderDto>>.Success(orders.Select(OrderDto.From).ToArray());
    }
    public async Task<ServiceResult<OrderDto>> LaunchAsync(int chefId, int orderId, CancellationToken ct)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        var order = await _db.Orders
            .FromSqlInterpolated($"SELECT * FROM \"Orders\" WHERE \"Id\" = {orderId} AND \"ChefId\" = {chefId} FOR UPDATE")
            .SingleOrDefaultAsync(ct);
        if (order is null) return ServiceResult<OrderDto>.Missing();
        if (order.Status != OrderStatus.Pending) return ServiceResult<OrderDto>.Conflicting("Only pending orders can be launched.");
        order.Status = OrderStatus.Launched;
        await _db.SaveChangesAsync(ct);
        await _db.Entry(order).Collection(x => x.Products).Query().Include(x => x.Product).LoadAsync(ct);
        await transaction.CommitAsync(ct);
        return ServiceResult<OrderDto>.Success(OrderDto.From(order));
    }
}
