using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Entities;

public class Restaurant
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    public DateTimeOffset DateOfAdd { get; set; } = DateTimeOffset.UtcNow;

    public RestaurantStatus Status { get; set; } = RestaurantStatus.Closed;

    public TimeOnly? TimeOpen { get; set; }

    public TimeOnly? TimeClose { get; set; }

    [StringLength(100)]
    public string? Nationality { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal Budget { get; set; }

    // Unique OwnerId should be enforced in the database to make this one-to-one.
    public int OwnerId { get; set; }

    [Required]
    public Owner Owner { get; set; } = null!;

    public ICollection<Chef> Chefs { get; set; } = new List<Chef>();

    public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();

    public ICollection<RestaurantOperatingDay> OperatingDays { get; set; } = new List<RestaurantOperatingDay>();

    public ICollection<RestaurantOffDay> OffDays { get; set; } = new List<RestaurantOffDay>();
}

public enum RestaurantStatus
{
    Closed = 0,
    Repair = 1,
    Open = 2
}
