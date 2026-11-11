using FoodDelivery.Api.Contracts.Candidatures;
using FoodDelivery.Api.Contracts.Chefs;
using FoodDelivery.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("api/restaurants/{restaurantId:int}/candidatures")]
public sealed class RestaurantCandidaturesController : ControllerBase
{
    private readonly ICandidatureService _candidatureService;
    private readonly ILogger<RestaurantCandidaturesController> _logger;

    public RestaurantCandidaturesController(ICandidatureService candidatureService,
        ILogger<RestaurantCandidaturesController> logger)
    {
        _candidatureService = candidatureService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CandidatureDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CandidatureDetailsDto>> Create(int restaurantId,
        [FromBody] CreateCandidatureRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received candidature for chef {ChefId} at restaurant {RestaurantId}",
            request.ChefId, restaurantId);
        var result = await _candidatureService.CreateAsync(
            restaurantId, request.ChefId, request.Salary, cancellationToken);
        if (!result.Succeeded)
        {
            return ControllerResultMapper.Map(this, result, _logger);
        }

        return CreatedAtAction(nameof(GetById),
            new { restaurantId, candidatureId = result.Value!.Id }, result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CandidatureDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<CandidatureDetailsDto>>> GetAll(int restaurantId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received candidature-list request for restaurant {RestaurantId}", restaurantId);
        return ControllerResultMapper.Map(this,
            await _candidatureService.GetRestaurantCandidaturesAsync(restaurantId, cancellationToken), _logger);
    }

    [HttpGet("{candidatureId:int}")]
    [ProducesResponseType(typeof(CandidatureDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CandidatureDetailsDto>> GetById(int restaurantId, int candidatureId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received lookup for candidature {CandidatureId} at restaurant {RestaurantId}",
            candidatureId, restaurantId);
        return ControllerResultMapper.Map(this,
            await _candidatureService.GetByIdAsync(restaurantId, candidatureId, cancellationToken), _logger);
    }

    [HttpGet("{candidatureId:int}/chef")]
    [ProducesResponseType(typeof(ChefDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChefDetailsDto>> GetChef(int restaurantId, int candidatureId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received chef lookup for candidature {CandidatureId} at restaurant {RestaurantId}",
            candidatureId, restaurantId);
        return ControllerResultMapper.Map(this,
            await _candidatureService.GetChefAsync(restaurantId, candidatureId, cancellationToken), _logger);
    }

    [HttpPut("{candidatureId:int}/refuse")]
    [ProducesResponseType(typeof(CandidatureDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CandidatureDetailsDto>> Refuse(int restaurantId, int candidatureId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received refusal request for candidature {CandidatureId} at restaurant {RestaurantId}",
            candidatureId, restaurantId);
        return ControllerResultMapper.Map(this,
            await _candidatureService.RefuseAsync(restaurantId, candidatureId, cancellationToken), _logger);
    }
}
