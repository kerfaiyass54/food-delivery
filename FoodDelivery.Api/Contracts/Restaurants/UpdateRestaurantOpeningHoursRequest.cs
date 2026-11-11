namespace FoodDelivery.Api.Contracts.Restaurants;

public sealed record UpdateRestaurantOpeningHoursRequest(TimeOnly OpensAt, TimeOnly ClosesAt);
