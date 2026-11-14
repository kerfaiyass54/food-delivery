using FoodDelivery.Api.Contracts.Commerce;
using FoodDelivery.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Api.Controllers;

[ApiController, Route("api/chefs/{chefId:int}/products")]
public sealed class ChefProductsController : ControllerBase
{
    private readonly IProductService _products;
    private readonly ILogger<ChefProductsController> _logger;
    public ChefProductsController(IProductService products, ILogger<ChefProductsController> logger) { _products = products; _logger = logger; }
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Cook(int chefId, [FromBody] CookProductRequest request, CancellationToken ct)
    {
        var result = await _products.CookAsync(chefId, request, ct);
        return result.Succeeded ? StatusCode(StatusCodes.Status201Created, result.Value) : ControllerResultMapper.Map(this, result, _logger);
    }
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> Get(int chefId, CancellationToken ct) =>
        ControllerResultMapper.Map(this, await _products.GetByChefAsync(chefId, ct), _logger);
    [HttpDelete("{productId:int}")]
    public async Task<ActionResult<object>> Delete(int chefId, int productId, CancellationToken ct)
    {
        var result = await _products.DeleteAsync(chefId, productId, ct);
        if (!result.Succeeded) return ControllerResultMapper.Map(this, result, _logger);
        return NoContent();
    }
}
