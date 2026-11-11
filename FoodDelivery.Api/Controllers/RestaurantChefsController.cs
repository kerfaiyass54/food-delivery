using FoodDelivery.Api.Contracts.Chefs;
using FoodDelivery.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("api/restaurants/{restaurantId:int}/chefs")]
public sealed class RestaurantChefsController : ControllerBase
{
    private readonly IChefService _chefService;
    private readonly ILogger<RestaurantChefsController> _logger;

    public RestaurantChefsController(IChefService chefService, ILogger<RestaurantChefsController> logger)
    {
        _chefService = chefService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ChefSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ChefSummaryDto>>> GetAll(int restaurantId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received chef-list request for restaurant {RestaurantId}", restaurantId);
        return ControllerResultMapper.Map(this,
            await _chefService.GetRestaurantChefsAsync(restaurantId, cancellationToken), _logger);
    }

    [HttpGet("{chefId:int}")]
    [ProducesResponseType(typeof(ChefDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChefDetailsDto>> GetDetails(int restaurantId, int chefId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received chef-detail request for chef {ChefId} at restaurant {RestaurantId}",
            chefId, restaurantId);
        return ControllerResultMapper.Map(this,
            await _chefService.GetRestaurantChefAsync(restaurantId, chefId, cancellationToken), _logger);
    }

    [HttpPost("{chefId:int}")]
    [ProducesResponseType(typeof(ChefDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ChefDetailsDto>> Add(int restaurantId, int chefId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to assign chef {ChefId} to restaurant {RestaurantId}",
            chefId, restaurantId);
        var result = await _chefService.AddChefToRestaurantAsync(restaurantId, chefId, cancellationToken);
        if (!result.Succeeded)
        {
            return ControllerResultMapper.Map(this, result, _logger);
        }

        return CreatedAtAction(nameof(GetDetails), new { restaurantId, chefId }, result.Value);
    }
}
