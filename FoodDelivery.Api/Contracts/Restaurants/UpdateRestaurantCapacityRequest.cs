using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Contracts.Restaurants;

public sealed record UpdateRestaurantCapacityRequest(
    [property: Range(1, int.MaxValue)] int Capacity);
