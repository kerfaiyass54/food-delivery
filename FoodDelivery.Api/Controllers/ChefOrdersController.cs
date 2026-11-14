using FoodDelivery.Api.Contracts.Commerce;
using FoodDelivery.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Api.Controllers;

[ApiController, Route("api/chefs/{chefId:int}/orders")]
public sealed class ChefOrdersController : ControllerBase
{
    private readonly IOrderService _orders;
    private readonly ILogger<ChefOrdersController> _logger;
    public ChefOrdersController(IOrderService orders, ILogger<ChefOrdersController> logger) { _orders = orders; _logger = logger; }
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> Get(int chefId, CancellationToken ct) =>
        ControllerResultMapper.Map(this, await _orders.GetByChefAsync(chefId, ct), _logger);
    [HttpPut("{orderId:int}/launch")]
    public async Task<ActionResult<OrderDto>> Launch(int chefId, int orderId, CancellationToken ct) =>
        ControllerResultMapper.Map(this, await _orders.LaunchAsync(chefId, orderId, ct), _logger);
}
