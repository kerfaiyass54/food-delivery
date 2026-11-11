using FoodDelivery.Api.Contracts.Chefs;
using FoodDelivery.Api.Data.Chefs;
using FoodDelivery.Api.Data.Restaurants;

namespace FoodDelivery.Api.Services;

public sealed class ChefService : IChefService
{
    private readonly IChefRepository _chefRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ILogger<ChefService> _logger;

    public ChefService(IChefRepository chefRepository, IRestaurantRepository restaurantRepository,
        ILogger<ChefService> logger)
    {
        _chefRepository = chefRepository;
        _restaurantRepository = restaurantRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<IReadOnlyList<ChefSummaryDto>>> GetRestaurantChefsAsync(int restaurantId,
        CancellationToken cancellationToken)
    {
        if (!await _restaurantRepository.RestaurantExistsAsync(restaurantId, cancellationToken))
        {
            _logger.LogInformation("Cannot load chefs; restaurant {RestaurantId} was not found", restaurantId);
            return ServiceResult<IReadOnlyList<ChefSummaryDto>>.Missing();
        }

        var chefs = await _chefRepository.GetByRestaurantIdAsync(restaurantId, cancellationToken);
        _logger.LogInformation("Loaded {ChefCount} chefs for restaurant {RestaurantId}", chefs.Count, restaurantId);
        return ServiceResult<IReadOnlyList<ChefSummaryDto>>.Success(chefs
            .Select(x => new ChefSummaryDto(x.Id, x.Name, x.Speciality, x.Experience)).ToArray());
    }

    public async Task<ServiceResult<ChefDetailsDto>> GetRestaurantChefAsync(int restaurantId, int chefId,
        CancellationToken cancellationToken)
    {
        var chef = await _chefRepository.GetByIdAsync(restaurantId, chefId, cancellationToken);
        if (chef is null)
        {
            _logger.LogInformation("Chef {ChefId} was not found in restaurant {RestaurantId}", chefId, restaurantId);
            return ServiceResult<ChefDetailsDto>.Missing();
        }

        _logger.LogInformation("Loaded details for chef {ChefId} at restaurant {RestaurantId}", chefId, restaurantId);
        return ServiceResult<ChefDetailsDto>.Success(ChefDetailsDto.FromEntity(chef));
    }

    public async Task<ServiceResult<ChefDetailsDto>> AddChefToRestaurantAsync(int restaurantId, int chefId,
        CancellationToken cancellationToken)
    {
        var assignment = await _chefRepository.AssignToRestaurantAsync(restaurantId, chefId, cancellationToken);
        if (assignment == ChefAssignmentStatus.RestaurantNotFound || assignment == ChefAssignmentStatus.ChefNotFound)
        {
            _logger.LogInformation("Could not assign chef {ChefId} to restaurant {RestaurantId}: {Assignment}",
                chefId, restaurantId, assignment);
            return ServiceResult<ChefDetailsDto>.Missing();
        }

        if (assignment == ChefAssignmentStatus.AlreadyAssigned)
        {
            return ServiceResult<ChefDetailsDto>.Conflicting("This chef already works at this restaurant.");
        }

        if (assignment == ChefAssignmentStatus.AssignedToAnotherRestaurant)
        {
            return ServiceResult<ChefDetailsDto>.Conflicting("This chef is already assigned to another restaurant.");
        }

        var chef = await _chefRepository.GetByIdAsync(restaurantId, chefId, cancellationToken);
        if (chef is null)
        {
            return ServiceResult<ChefDetailsDto>.Missing();
        }

        _logger.LogInformation("Added chef {ChefId} to restaurant {RestaurantId}", chefId, restaurantId);
        return ServiceResult<ChefDetailsDto>.Success(ChefDetailsDto.FromEntity(chef));
    }
}
