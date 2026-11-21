using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Entities;

public class Product
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Type { get; set; } = string.Empty;
    [Range(0, int.MaxValue)] public int Stock { get; set; }
    [Range(typeof(decimal), "0.01", "9999999999999999.99")] public decimal? Price { get; set; }
    public DateTimeOffset CookDate { get; set; } = DateTimeOffset.UtcNow;
    public int ChefId { get; set; }
    [Required] public Chef Chef { get; set; } = null!;
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
