using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Contracts.Chefs;

public sealed record UpdateChefProfileRequest(
    [Range(0, 80)] int Experience,
    [Required, StringLength(100)] string Speciality,
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] decimal Salary);
