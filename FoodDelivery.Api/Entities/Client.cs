using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Entities;

public class Client
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(100)] public string City { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Nationality { get; set; } = string.Empty;
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal Budget { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal Weight { get; set; }
    public ClientStatus Status { get; set; } = ClientStatus.Normal;
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<FoodOrder> Orders { get; set; } = new List<FoodOrder>();
}

public enum ClientStatus { Sick = 0, Normal = 1, Diet = 2 }
