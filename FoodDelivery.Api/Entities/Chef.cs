using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Entities;

public class Chef
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Speciality { get; set; } = string.Empty;

    [Range(0, 80)]
    public int Experience { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal Salary { get; set; }

    // Required if a chef belongs to a restaurant; remove this FK if chefs are independent applicants.
    public int? RestaurantId { get; set; }

    public Restaurant? Restaurant { get; set; }

    public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();
}
