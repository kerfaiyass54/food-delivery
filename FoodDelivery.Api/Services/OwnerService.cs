using FoodDelivery.Api.Contracts.Restaurants;
using FoodDelivery.Api.Data.Restaurants;

namespace FoodDelivery.Api.Services;

public sealed class OwnerService : IOwnerService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ILogger<OwnerService> _logger;

    public OwnerService(IRestaurantRepository restaurantRepository, ILogger<OwnerService> logger)
    {
        _restaurantRepository = restaurantRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<RestaurantDetailsDto>> GetManagedRestaurantAsync(int ownerId,
        CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        if (restaurant is null)
        {
            _logger.LogInformation("No restaurant is managed by owner {OwnerId}", ownerId);
            return ServiceResult<RestaurantDetailsDto>.Missing();
        }

        _logger.LogInformation("Loaded restaurant {RestaurantId} managed by owner {OwnerId}",
            restaurant.Id, ownerId);
        return ServiceResult<RestaurantDetailsDto>.Success(RestaurantDetailsDto.FromEntity(restaurant));
    }
}
