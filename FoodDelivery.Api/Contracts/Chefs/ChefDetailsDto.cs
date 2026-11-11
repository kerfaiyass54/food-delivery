using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Contracts.Chefs;

public sealed record ChefDetailsDto(
    int Id,
    string Name,
    string Speciality,
    int Experience,
    decimal Salary,
    int? RestaurantId)
{
    public static ChefDetailsDto FromEntity(Chef chef) => new(
        chef.Id, chef.Name, chef.Speciality, chef.Experience, chef.Salary, chef.RestaurantId);
}
