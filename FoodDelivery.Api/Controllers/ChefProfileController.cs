using FoodDelivery.Api.Contracts.Chefs;
using FoodDelivery.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("api/chefs/{chefId:int}")]
public sealed class ChefProfileController : ControllerBase
{
    private readonly IChefProfileService _service;
    private readonly ILogger<ChefProfileController> _logger;
    public ChefProfileController(IChefProfileService service, ILogger<ChefProfileController> logger)
    { _service = service; _logger = logger; }

    [HttpPut("profile")]
    [ProducesResponseType(typeof(ChefDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChefDetailsDto>> UpdateProfile(int chefId,
        [FromBody] UpdateChefProfileRequest request, CancellationToken cancellationToken) =>
        ControllerResultMapper.Map(this, await _service.UpdateAsync(chefId, request, cancellationToken), _logger);
}
