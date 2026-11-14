using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Contracts.Candidatures;

public sealed record CreateChefCandidatureRequest(
    [Range(1, int.MaxValue)] int RestaurantId,
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] decimal Salary);
