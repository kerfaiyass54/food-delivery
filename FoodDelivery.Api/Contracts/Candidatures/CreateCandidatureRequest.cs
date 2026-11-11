using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Contracts.Candidatures;

public sealed record CreateCandidatureRequest(
    [property: Range(1, int.MaxValue)] int ChefId,
    [property: Range(typeof(decimal), "0", "79228162514264337593543950335")] decimal Salary);
