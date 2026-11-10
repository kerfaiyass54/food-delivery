using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Contracts.Restaurants;

public sealed record DayDto(int Id, string Name);

public sealed record RestaurantDetailsDto(
    int Id,
    string Name,
    string Address,
    int Capacity,
    DateTimeOffset DateOfAdd,
    string Status,
    TimeOnly? TimeOpen,
    TimeOnly? TimeClose,
    string? Nationality,
    decimal Budget,
    IReadOnlyCollection<DayDto> OperatingDays,
    IReadOnlyCollection<DayDto> OffDays)
{
    public static RestaurantDetailsDto FromEntity(Restaurant restaurant) => new(
        restaurant.Id,
        restaurant.Name,
        restaurant.Address,
        restaurant.Capacity,
        restaurant.DateOfAdd,
        restaurant.Status.ToString(),
        restaurant.TimeOpen,
        restaurant.TimeClose,
        restaurant.Nationality,
        restaurant.Budget,
        restaurant.OperatingDays.OrderBy(x => x.DayId)
            .Select(x => new DayDto(x.DayId, x.Day.Name)).ToArray(),
        restaurant.OffDays.OrderBy(x => x.DayId)
            .Select(x => new DayDto(x.DayId, x.Day.Name)).ToArray());
}
