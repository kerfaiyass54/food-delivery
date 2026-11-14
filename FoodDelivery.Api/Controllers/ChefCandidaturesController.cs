using FoodDelivery.Api.Contracts.Candidatures;
using FoodDelivery.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("api/chefs/{chefId:int}/candidatures")]
public sealed class ChefCandidaturesController : ControllerBase
{
    private readonly IChefCandidatureService _service;
    private readonly ILogger<ChefCandidaturesController> _logger;
    public ChefCandidaturesController(IChefCandidatureService service, ILogger<ChefCandidaturesController> logger)
    { _service = service; _logger = logger; }

    [HttpPost]
    [ProducesResponseType(typeof(CandidatureDetailsDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CandidatureDetailsDto>> Create(int chefId,
        [FromBody] CreateChefCandidatureRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(chefId, request, cancellationToken);
        if (!result.Succeeded) return ControllerResultMapper.Map(this, result, _logger);
        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CandidatureDetailsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CandidatureDetailsDto>>> GetAll(int chefId,
        CancellationToken cancellationToken) =>
        ControllerResultMapper.Map(this, await _service.GetAllAsync(chefId, cancellationToken), _logger);

    [HttpDelete("{candidatureId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> Delete(int chefId, int candidatureId, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(chefId, candidatureId, cancellationToken);
        if (!result.Succeeded) return ControllerResultMapper.Map(this, result, _logger);
        return NoContent();
    }
}
