using FoodDelivery.Api.Contracts.Commerce;
using FoodDelivery.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Api.Controllers;

[ApiController, Route("api/clients")]
public sealed class ClientsController : ControllerBase
{
    private readonly IClientService _clients;
    private readonly IPurchaseService _purchases;
    private readonly IOrderService _orders;
    private readonly ILogger<ClientsController> _logger;
    public ClientsController(IClientService clients, IPurchaseService purchases, IOrderService orders,
        ILogger<ClientsController> logger) { _clients = clients; _purchases = purchases; _orders = orders; _logger = logger; }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create([FromBody] CreateClientRequest request, CancellationToken ct)
    {
        var result = await _clients.CreateAsync(request, ct);
        return result.Succeeded ? StatusCode(StatusCodes.Status201Created, result.Value) : ControllerResultMapper.Map(this, result, _logger);
    }
    [HttpGet("products")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> Products(CancellationToken ct) =>
        ControllerResultMapper.Map(this, await _clients.GetProductsAsync(ct), _logger);
    [HttpPost("{clientId:int}/purchases")]
    public async Task<ActionResult<PurchaseDto>> Purchase(int clientId, [FromBody] PurchaseRequest request, CancellationToken ct)
    {
        var result = await _purchases.PurchaseAsync(clientId, request, ct);
        return result.Succeeded ? StatusCode(StatusCodes.Status201Created, result.Value) : ControllerResultMapper.Map(this, result, _logger);
    }
    [HttpGet("{clientId:int}/purchases")]
    public async Task<ActionResult<IReadOnlyList<PurchaseDto>>> Purchases(int clientId, CancellationToken ct) =>
        ControllerResultMapper.Map(this, await _purchases.GetByClientAsync(clientId, ct), _logger);
    [HttpPost("{clientId:int}/orders")]
    public async Task<ActionResult<OrderDto>> Order(int clientId, [FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var result = await _orders.CreateAsync(clientId, request, ct);
        return result.Succeeded ? StatusCode(StatusCodes.Status201Created, result.Value) : ControllerResultMapper.Map(this, result, _logger);
    }
    [HttpGet("{clientId:int}/orders")]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> Orders(int clientId, CancellationToken ct) =>
        ControllerResultMapper.Map(this, await _clients.GetOrdersAsync(clientId, ct), _logger);
    [HttpDelete("{clientId:int}/orders/{orderId:int}")]
    public async Task<ActionResult<object>> CancelOrder(int clientId, int orderId, CancellationToken ct)
    {
        var result = await _clients.CancelOrderAsync(clientId, orderId, ct);
        if (!result.Succeeded) return ControllerResultMapper.Map(this, result, _logger);
        return NoContent();
    }
}
