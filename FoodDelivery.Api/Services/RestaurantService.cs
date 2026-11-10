using FoodDelivery.Api.Contracts.Restaurants;
using FoodDelivery.Api.Data.Restaurants;

namespace FoodDelivery.Api.Services;

public sealed class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ILogger<RestaurantService> _logger;

    public RestaurantService(IRestaurantRepository restaurantRepository, ILogger<RestaurantService> logger)
    {
        _restaurantRepository = restaurantRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<RestaurantDetailsDto>> GetByIdAsync(int restaurantId,
        CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant is null)
        {
            _logger.LogInformation("Restaurant {RestaurantId} was not found", restaurantId);
            return ServiceResult<RestaurantDetailsDto>.Missing();
        }

        _logger.LogInformation("Loaded restaurant {RestaurantId}", restaurantId);
        return ServiceResult<RestaurantDetailsDto>.Success(RestaurantDetailsDto.FromEntity(restaurant));
    }

    public async Task<ServiceResult<RestaurantDetailsDto>> UpdateCapacityAsync(int restaurantId, int capacity,
        CancellationToken cancellationToken)
    {
        if (capacity <= 0)
        {
            return ServiceResult<RestaurantDetailsDto>.Invalid("Capacity must be greater than zero.");
        }

        if (!await _restaurantRepository.UpdateCapacityAsync(restaurantId, capacity, cancellationToken))
        {
            _logger.LogInformation("Cannot update capacity; restaurant {RestaurantId} was not found", restaurantId);
            return ServiceResult<RestaurantDetailsDto>.Missing();
        }

        _logger.LogInformation("Updated capacity for restaurant {RestaurantId} to {Capacity}", restaurantId, capacity);
        return await GetByIdAsync(restaurantId, cancellationToken);
    }

    public async Task<ServiceResult<RestaurantDetailsDto>> UpdateDaysAsync(int restaurantId,
        int[] operatingDayIds, int[] offDayIds, CancellationToken cancellationToken)
    {
        if (operatingDayIds is null || offDayIds is null)
        {
            return ServiceResult<RestaurantDetailsDto>.Invalid("OperatingDayIds and OffDayIds are required.");
        }

        if (operatingDayIds.Distinct().Count() != operatingDayIds.Length ||
            offDayIds.Distinct().Count() != offDayIds.Length)
        {
            return ServiceResult<RestaurantDetailsDto>.Invalid("A day cannot appear more than once in either list.");
        }

        if (operatingDayIds.Intersect(offDayIds).Any())
        {
            return ServiceResult<RestaurantDetailsDto>.Invalid("A day cannot be both an operating day and a day off.");
        }

        var allDayIds = operatingDayIds.Concat(offDayIds).ToArray();
        if (!await _restaurantRepository.AllDaysExistAsync(allDayIds, cancellationToken))
        {
            return ServiceResult<RestaurantDetailsDto>.Invalid("One or more day IDs do not exist. Valid IDs are 1 through 7.");
        }

        if (!await _restaurantRepository.UpdateDaysAsync(restaurantId, operatingDayIds, offDayIds, cancellationToken))
        {
            _logger.LogInformation("Cannot update days; restaurant {RestaurantId} was not found", restaurantId);
            return ServiceResult<RestaurantDetailsDto>.Missing();
        }

        _logger.LogInformation("Updated operating days and days off for restaurant {RestaurantId}", restaurantId);
        return await GetByIdAsync(restaurantId, cancellationToken);
    }

    public async Task<ServiceResult<RestaurantDetailsDto>> UpdateOpeningHoursAsync(int restaurantId,
        TimeOnly opensAt, TimeOnly closesAt, CancellationToken cancellationToken)
    {
        if (closesAt <= opensAt)
        {
            return ServiceResult<RestaurantDetailsDto>.Invalid("ClosesAt must be later than OpensAt on the same day.");
        }

        if (!await _restaurantRepository.UpdateOpeningHoursAsync(restaurantId, opensAt, closesAt, cancellationToken))
        {
            _logger.LogInformation("Cannot update opening hours; restaurant {RestaurantId} was not found", restaurantId);
            return ServiceResult<RestaurantDetailsDto>.Missing();
        }

        _logger.LogInformation("Updated opening hours for restaurant {RestaurantId}: {OpensAt} to {ClosesAt}",
            restaurantId, opensAt, closesAt);
        return await GetByIdAsync(restaurantId, cancellationToken);
    }
}
