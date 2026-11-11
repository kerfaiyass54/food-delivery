using FoodDelivery.Api.Contracts.Restaurants;

namespace FoodDelivery.Api.Services;

public interface IOwnerService
{
    Task<ServiceResult<RestaurantDetailsDto>> GetManagedRestaurantAsync(int ownerId,
        CancellationToken cancellationToken);
}
