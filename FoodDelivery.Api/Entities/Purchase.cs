using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Entities;

public class Purchase
{
    public int Id { get; set; }
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal Price { get; set; }
    [Range(1, int.MaxValue)] public int Quantity { get; set; }
    public int ClientId { get; set; }
    [Required] public Client Client { get; set; } = null!;
    public int ProductId { get; set; }
    [Required] public Product Product { get; set; } = null!;
}
