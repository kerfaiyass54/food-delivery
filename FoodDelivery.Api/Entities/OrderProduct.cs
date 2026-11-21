using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Entities;

public class OrderProduct
{
    public int OrderId { get; set; }
    [Required] public FoodOrder Order { get; set; } = null!;
    public int ProductId { get; set; }
    [Required] public Product Product { get; set; } = null!;
    [Range(1, int.MaxValue)] public int Quantity { get; set; }
}
