using FoodDelivery.Api.Contracts.Restaurants;

namespace FoodDelivery.Api.Services;

public interface IRestaurantService
{
    Task<ServiceResult<RestaurantDetailsDto>> GetByIdAsync(int restaurantId, CancellationToken cancellationToken);
    Task<ServiceResult<RestaurantDetailsDto>> UpdateCapacityAsync(int restaurantId, int capacity,
        CancellationToken cancellationToken);
    Task<ServiceResult<RestaurantDetailsDto>> UpdateDaysAsync(int restaurantId, int[] operatingDayIds,
        int[] offDayIds, CancellationToken cancellationToken);
    Task<ServiceResult<RestaurantDetailsDto>> UpdateOpeningHoursAsync(int restaurantId, TimeOnly opensAt,
        TimeOnly closesAt, CancellationToken cancellationToken);
}
