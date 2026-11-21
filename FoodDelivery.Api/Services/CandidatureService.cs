using FoodDelivery.Api.Contracts.Candidatures;
using FoodDelivery.Api.Contracts.Chefs;
using FoodDelivery.Api.Data;
using FoodDelivery.Api.Data.Candidatures;
using FoodDelivery.Api.Data.Chefs;
using FoodDelivery.Api.Data.Restaurants;
using FoodDelivery.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Services;

public sealed class CandidatureService : ICandidatureService
{
    private readonly ICandidatureRepository _candidatureRepository;
    private readonly IChefRepository _chefRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly FoodDeliveryDbContext _dbContext;
    private readonly ILogger<CandidatureService> _logger;

    public CandidatureService(ICandidatureRepository candidatureRepository,
        IChefRepository chefRepository, IRestaurantRepository restaurantRepository,
        FoodDeliveryDbContext dbContext, ILogger<CandidatureService> logger)
    {
        _candidatureRepository = candidatureRepository;
        _chefRepository = chefRepository;
        _restaurantRepository = restaurantRepository;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ServiceResult<CandidatureDetailsDto>> CreateAsync(int restaurantId, int chefId,
        decimal salary, CancellationToken cancellationToken)
    {
        if (salary < 0)
        {
            return ServiceResult<CandidatureDetailsDto>.Invalid("Salary cannot be negative.");
        }

        var creation = await _candidatureRepository.AddAsync(restaurantId, chefId, salary, cancellationToken);
        if (creation.Status == CandidatureCreationStatus.RestaurantNotFound ||
            creation.Status == CandidatureCreationStatus.ChefNotFound)
        {
            _logger.LogInformation("Could not create candidature for chef {ChefId} at restaurant {RestaurantId}: {Result}",
                chefId, restaurantId, creation.Status);
            return ServiceResult<CandidatureDetailsDto>.Missing();
        }

        var created = creation.CandidatureId is int candidatureId
            ? await _candidatureRepository.GetByIdAsync(restaurantId, candidatureId, cancellationToken)
            : null;
        if (created is null)
        {
            _logger.LogError("Created candidature for chef {ChefId} at restaurant {RestaurantId} could not be reloaded",
                chefId, restaurantId);
            return ServiceResult<CandidatureDetailsDto>.Missing();
        }

        _logger.LogInformation("Created candidature {CandidatureId} for chef {ChefId} at restaurant {RestaurantId}",
            created.Id, chefId, restaurantId);
        return ServiceResult<CandidatureDetailsDto>.Success(CandidatureDetailsDto.FromEntity(created));
    }

    public async Task<ServiceResult<IReadOnlyList<CandidatureDetailsDto>>> GetRestaurantCandidaturesAsync(
        int restaurantId, CancellationToken cancellationToken)
    {
        if (!await _restaurantRepository.RestaurantExistsAsync(restaurantId, cancellationToken))
        {
            _logger.LogInformation("Cannot load candidatures; restaurant {RestaurantId} was not found", restaurantId);
            return ServiceResult<IReadOnlyList<CandidatureDetailsDto>>.Missing();
        }

        var candidatures = await _candidatureRepository.GetByRestaurantIdAsync(restaurantId, cancellationToken);
        _logger.LogInformation("Loaded {CandidatureCount} candidatures for restaurant {RestaurantId}",
            candidatures.Count, restaurantId);
        return ServiceResult<IReadOnlyList<CandidatureDetailsDto>>.Success(
            candidatures.Select(CandidatureDetailsDto.FromEntity).ToArray());
    }

    public async Task<ServiceResult<CandidatureDetailsDto>> GetByIdAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken)
    {
        var candidature = await _candidatureRepository.GetByIdAsync(restaurantId, candidatureId, cancellationToken);
        if (candidature is null)
        {
            _logger.LogInformation("Candidature {CandidatureId} was not found for restaurant {RestaurantId}",
                candidatureId, restaurantId);
            return ServiceResult<CandidatureDetailsDto>.Missing();
        }

        return ServiceResult<CandidatureDetailsDto>.Success(CandidatureDetailsDto.FromEntity(candidature));
    }

    public async Task<ServiceResult<ChefDetailsDto>> GetChefAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken)
    {
        var candidature = await _candidatureRepository.GetByIdAsync(restaurantId, candidatureId, cancellationToken);
        if (candidature is null)
        {
            _logger.LogInformation("Candidature {CandidatureId} was not found for restaurant {RestaurantId}",
                candidatureId, restaurantId);
            return ServiceResult<ChefDetailsDto>.Missing();
        }

        _logger.LogInformation("Loaded chef {ChefId} related to candidature {CandidatureId}",
            candidature.ChefId, candidatureId);
        return ServiceResult<ChefDetailsDto>.Success(ChefDetailsDto.FromEntity(candidature.Chef));
    }

    public async Task<ServiceResult<CandidatureDetailsDto>> AcceptAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var candidature = await _candidatureRepository.GetByIdAsync(
            restaurantId, candidatureId, cancellationToken);
        if (candidature is null)
        {
            return ServiceResult<CandidatureDetailsDto>.Missing();
        }

        if (candidature.Status != CandidatureStatus.Pending)
        {
            return ServiceResult<CandidatureDetailsDto>.Conflicting("Only pending candidatures can be accepted.");
        }

        var assignment = await _chefRepository.AssignToRestaurantAsync(
            restaurantId, candidature.ChefId, cancellationToken);
        if (assignment is ChefAssignmentStatus.RestaurantNotFound or ChefAssignmentStatus.ChefNotFound)
        {
            return ServiceResult<CandidatureDetailsDto>.Missing();
        }

        if (assignment is ChefAssignmentStatus.AlreadyAssigned or ChefAssignmentStatus.AssignedToAnotherRestaurant)
        {
            return ServiceResult<CandidatureDetailsDto>.Conflicting(
                assignment == ChefAssignmentStatus.AlreadyAssigned
                    ? "This chef already works at this restaurant."
                    : "This chef is already assigned to another restaurant.");
        }

        var decision = await _candidatureRepository.AcceptAsync(
            restaurantId, candidatureId, cancellationToken);
        if (decision == CandidatureDecisionStatus.NotFound)
        {
            return ServiceResult<CandidatureDetailsDto>.Missing();
        }

        if (decision == CandidatureDecisionStatus.AlreadyDecided)
        {
            return ServiceResult<CandidatureDetailsDto>.Conflicting("Only pending candidatures can be accepted.");
        }

        var accepted = await _candidatureRepository.GetByIdAsync(
            restaurantId, candidatureId, cancellationToken);
        if (accepted is null)
        {
            return ServiceResult<CandidatureDetailsDto>.Missing();
        }

        await transaction.CommitAsync(cancellationToken);
        _logger.LogInformation("Accepted candidature {CandidatureId} for restaurant {RestaurantId}",
            candidatureId, restaurantId);
        return ServiceResult<CandidatureDetailsDto>.Success(CandidatureDetailsDto.FromEntity(accepted));
    }

    public async Task<ServiceResult<CandidatureDetailsDto>> RefuseAsync(int restaurantId, int candidatureId,
        CancellationToken cancellationToken)
    {
        var decision = await _candidatureRepository.RefuseAsync(restaurantId, candidatureId, cancellationToken);
        if (decision == CandidatureDecisionStatus.NotFound)
        {
            _logger.LogInformation("Candidature {CandidatureId} was not found for restaurant {RestaurantId}",
                candidatureId, restaurantId);
            return ServiceResult<CandidatureDetailsDto>.Missing();
        }

        if (decision == CandidatureDecisionStatus.AlreadyDecided)
        {
            _logger.LogWarning("Candidature {CandidatureId} for restaurant {RestaurantId} is not pending",
                candidatureId, restaurantId);
            return ServiceResult<CandidatureDetailsDto>.Conflicting("Only pending candidatures can be refused.");
        }

        var refused = await _candidatureRepository.GetByIdAsync(restaurantId, candidatureId, cancellationToken);
        if (refused is null)
        {
            return ServiceResult<CandidatureDetailsDto>.Missing();
        }

        _logger.LogInformation("Refused candidature {CandidatureId} for restaurant {RestaurantId}",
            candidatureId, restaurantId);
        return ServiceResult<CandidatureDetailsDto>.Success(CandidatureDetailsDto.FromEntity(refused));
    }
}
