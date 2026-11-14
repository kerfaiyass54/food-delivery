using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Data.Restaurants;

public interface IRestaurantRepository
{
    Task<Restaurant?> GetByIdAsync(int restaurantId, CancellationToken cancellationToken);
    Task<Restaurant?> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken);
    Task<bool> RestaurantExistsAsync(int restaurantId, CancellationToken cancellationToken);
    Task<bool> AllDaysExistAsync(IReadOnlyCollection<int> dayIds, CancellationToken cancellationToken);
    Task<bool> UpdateCapacityAsync(int restaurantId, int capacity, CancellationToken cancellationToken);
    Task<bool> UpdateDaysAsync(int restaurantId, IReadOnlyCollection<int> operatingDayIds,
        IReadOnlyCollection<int> offDayIds, CancellationToken cancellationToken);
    Task<bool> UpdateOpeningHoursAsync(int restaurantId, TimeOnly opensAt, TimeOnly closesAt,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Restaurant>> SearchAsync(string? filter, int page, int pageSize, string sortBy,
        bool descending, CancellationToken cancellationToken);
    Task<int> CountSearchAsync(string? filter, CancellationToken cancellationToken);
}
