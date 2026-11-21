using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Contracts.Candidatures;

public sealed record CandidatureDetailsDto(
    int Id,
    int ChefId,
    string ChefName,
    string ChefSpeciality,
    int ChefExperience,
    decimal Salary,
    DateTimeOffset Date,
    string Status,
    int RestaurantId,
    string RestaurantName,
    string Address)
{
    public static CandidatureDetailsDto FromEntity(Candidature candidature) => new(
        candidature.Id,
        candidature.ChefId,
        candidature.Chef.Name,
        candidature.Chef.Speciality,
        candidature.Chef.Experience,
        candidature.Salary,
        candidature.Date,
        candidature.Status.ToString(),
        candidature.RestaurantId,
        candidature.Restaurant.Name,
        candidature.Restaurant.Address);
}
