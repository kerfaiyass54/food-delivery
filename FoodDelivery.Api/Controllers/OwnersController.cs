using FoodDelivery.Api.Contracts.Restaurants;
using FoodDelivery.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("api/owners")]
public sealed class OwnersController : ControllerBase
{
    private readonly IOwnerService _ownerService;
    private readonly ILogger<OwnersController> _logger;

    public OwnersController(IOwnerService ownerService, ILogger<OwnersController> logger)
    {
        _ownerService = ownerService;
        _logger = logger;
    }

    [HttpGet("{ownerId:int}/restaurant")]
    [ProducesResponseType(typeof(RestaurantDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestaurantDetailsDto>> GetManagedRestaurant(int ownerId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received managed-restaurant lookup for owner {OwnerId}", ownerId);
        var result = await _ownerService.GetManagedRestaurantAsync(ownerId, cancellationToken);

        if (result.NotFound)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Managed restaurant not found",
                Detail = $"Owner {ownerId} does not have a restaurant.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(result.Value);
    }
}
