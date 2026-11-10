using FoodDelivery.Api.Contracts.Restaurants;
using FoodDelivery.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("api/restaurants")]
public sealed class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;
    private readonly ILogger<RestaurantsController> _logger;

    public RestaurantsController(IRestaurantService restaurantService, ILogger<RestaurantsController> logger)
    {
        _restaurantService = restaurantService;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RestaurantDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestaurantDetailsDto>> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received restaurant lookup for {RestaurantId}", id);
        return ToActionResult(await _restaurantService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPut("{id:int}/capacity")]
    [ProducesResponseType(typeof(RestaurantDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestaurantDetailsDto>> UpdateCapacity(int id,
        [FromBody] UpdateRestaurantCapacityRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received capacity update for restaurant {RestaurantId}", id);
        return ToActionResult(await _restaurantService.UpdateCapacityAsync(id, request.Capacity, cancellationToken));
    }

    [HttpPut("{id:int}/days")]
    [ProducesResponseType(typeof(RestaurantDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestaurantDetailsDto>> UpdateDays(int id,
        [FromBody] UpdateRestaurantDaysRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received schedule-day update for restaurant {RestaurantId}", id);
        return ToActionResult(await _restaurantService.UpdateDaysAsync(
            id, request.OperatingDayIds, request.OffDayIds, cancellationToken));
    }

    [HttpPut("{id:int}/opening-hours")]
    [ProducesResponseType(typeof(RestaurantDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestaurantDetailsDto>> UpdateOpeningHours(int id,
        [FromBody] UpdateRestaurantOpeningHoursRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received opening-hours update for restaurant {RestaurantId}", id);
        return ToActionResult(await _restaurantService.UpdateOpeningHoursAsync(
            id, request.OpensAt, request.ClosesAt, cancellationToken));
    }

    private ActionResult<RestaurantDetailsDto> ToActionResult(ServiceResult<RestaurantDetailsDto> result)
    {
        if (result.NotFound)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Restaurant not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        if (!result.Succeeded)
        {
            _logger.LogWarning("Restaurant request rejected: {Error}", result.Error);
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid restaurant request",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Ok(result.Value);
    }
}
