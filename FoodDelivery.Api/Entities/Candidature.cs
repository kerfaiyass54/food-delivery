using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Entities;

public class Candidature
{
    public int Id { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal Salary { get; set; }

    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

    public CandidatureStatus Status { get; set; } = CandidatureStatus.Pending;

    public int ChefId { get; set; }

    [Required]
    public Chef Chef { get; set; } = null!;

    public int RestaurantId { get; set; }

    [Required]
    public Restaurant Restaurant { get; set; } = null!;
}

public enum CandidatureStatus
{
    Pending = 0,
    Accepted = 1,
    Refused = 2
}
