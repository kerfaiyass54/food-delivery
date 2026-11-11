using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Contracts.Restaurants;

public sealed record UpdateRestaurantDaysRequest(
    [property: Required] int[] OperatingDayIds,
    [property: Required] int[] OffDayIds);
