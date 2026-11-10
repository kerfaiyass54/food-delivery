using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Entities;

public class Owner
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(18, 120)]
    public int Age { get; set; }

    [Required, Phone, StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    public Restaurant? Restaurant { get; set; }
}
