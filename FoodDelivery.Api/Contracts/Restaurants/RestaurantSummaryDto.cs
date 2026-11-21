using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Contracts.Restaurants;

public sealed record RestaurantSummaryDto(
    int Id,
    string Name,
    string Address,
    int Capacity,
    string Status,
    string? Nationality,
    decimal Budget,
    TimeOnly? TimeOpen,
    TimeOnly? TimeClose,
    IReadOnlyList<string> OperatingDays)
{
    public static RestaurantSummaryDto FromEntity(Restaurant restaurant) =>
        new(restaurant.Id, restaurant.Name, restaurant.Address, restaurant.Capacity,
            restaurant.Status.ToString(), restaurant.Nationality, restaurant.Budget,
            restaurant.TimeOpen, restaurant.TimeClose,
            restaurant.OperatingDays.OrderBy(day => day.DayId).Select(day => day.Day.Name).ToArray());
}

public sealed record PagedResultDto<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount,
    int TotalPages);
