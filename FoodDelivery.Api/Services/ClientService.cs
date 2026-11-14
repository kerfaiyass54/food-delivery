using FoodDelivery.Api.Contracts.Commerce;
using FoodDelivery.Api.Data;
using FoodDelivery.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Services;

public interface IClientService
{
    Task<ServiceResult<ClientDto>> CreateAsync(CreateClientRequest request, CancellationToken ct);
    Task<ServiceResult<IReadOnlyList<ProductDto>>> GetProductsAsync(CancellationToken ct);
    Task<ServiceResult<IReadOnlyList<OrderDto>>> GetOrdersAsync(int clientId, CancellationToken ct);
    Task<ServiceResult<object>> CancelOrderAsync(int clientId, int orderId, CancellationToken ct);
}

public sealed class ClientService : IClientService
{
    private readonly FoodDeliveryDbContext _db;
    public ClientService(FoodDeliveryDbContext db) => _db = db;
    public async Task<ServiceResult<ClientDto>> CreateAsync(CreateClientRequest request, CancellationToken ct)
    {
        if (!Enum.IsDefined(typeof(ClientStatus), request.Status)) return ServiceResult<ClientDto>.Invalid("Invalid client status.");
        var client = new Client { Name = request.Name.Trim(), City = request.City.Trim(), Nationality = request.Nationality.Trim(), Budget = request.Budget, Weight = request.Weight, Status = request.Status };
        _db.Clients.Add(client); await _db.SaveChangesAsync(ct);
        return ServiceResult<ClientDto>.Success(ClientDto.From(client));
    }
    public async Task<ServiceResult<IReadOnlyList<ProductDto>>> GetProductsAsync(CancellationToken ct)
    {
        var products = await _db.Products.AsNoTracking().Where(x => x.Stock > 0).OrderBy(x => x.Name).ToListAsync(ct);
        return ServiceResult<IReadOnlyList<ProductDto>>.Success(products.Select(ProductDto.From).ToArray());
    }
    public async Task<ServiceResult<IReadOnlyList<OrderDto>>> GetOrdersAsync(int clientId, CancellationToken ct)
    {
        if (!await _db.Clients.AnyAsync(x => x.Id == clientId, ct)) return ServiceResult<IReadOnlyList<OrderDto>>.Missing();
        var orders = await _db.Orders.AsNoTracking().Include(x => x.Products).ThenInclude(x => x.Product)
            .Where(x => x.ClientId == clientId).OrderByDescending(x => x.Date).ToListAsync(ct);
        return ServiceResult<IReadOnlyList<OrderDto>>.Success(orders.Select(OrderDto.From).ToArray());
    }
    public async Task<ServiceResult<object>> CancelOrderAsync(int clientId, int orderId, CancellationToken ct)
    {
        var order = await _db.Orders.Include(x => x.Products).ThenInclude(x => x.Product)
            .SingleOrDefaultAsync(x => x.Id == orderId && x.ClientId == clientId, ct);
        if (order is null) return ServiceResult<object>.Missing();
        if (order.Status != OrderStatus.Pending) return ServiceResult<object>.Conflicting("Only pending orders can be cancelled.");
        order.Status = OrderStatus.Cancelled;
        foreach (var line in order.Products) line.Product.Stock += line.Quantity;
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.Success(new { order.Id, Status = order.Status.ToString() });
    }
}
