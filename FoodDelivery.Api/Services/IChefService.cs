using FoodDelivery.Api.Contracts.Chefs;

namespace FoodDelivery.Api.Services;

public interface IChefService
{
    Task<ServiceResult<IReadOnlyList<ChefSummaryDto>>> GetRestaurantChefsAsync(int restaurantId,
        CancellationToken cancellationToken);
    Task<ServiceResult<ChefDetailsDto>> GetRestaurantChefAsync(int restaurantId, int chefId,
        CancellationToken cancellationToken);
    Task<ServiceResult<ChefDetailsDto>> AddChefToRestaurantAsync(int restaurantId, int chefId,
        CancellationToken cancellationToken);
}
