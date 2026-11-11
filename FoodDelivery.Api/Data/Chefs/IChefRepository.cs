using FoodDelivery.Api.Entities;
using FoodDelivery.Api.Data.Restaurants;

namespace FoodDelivery.Api.Data.Chefs;

public interface IChefRepository
{
    Task<IReadOnlyList<Chef>> GetByRestaurantIdAsync(int restaurantId, CancellationToken cancellationToken);
    Task<Chef?> GetByIdAsync(int restaurantId, int chefId, CancellationToken cancellationToken);
    Task<ChefAssignmentStatus> AssignToRestaurantAsync(int restaurantId, int chefId,
        CancellationToken cancellationToken);
}
