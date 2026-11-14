using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Contracts.Restaurants;

public sealed record RestaurantSummaryDto(int Id, string Name, string Address, string? Nationality, decimal Budget)
{
    public static RestaurantSummaryDto FromEntity(Restaurant restaurant) =>
        new(restaurant.Id, restaurant.Name, restaurant.Address, restaurant.Nationality, restaurant.Budget);
}

public sealed record PagedResultDto<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount,
    int TotalPages);
