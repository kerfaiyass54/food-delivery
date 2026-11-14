using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Entities;

public class FoodOrder
{
    public int Id { get; set; }
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
    public TimeOnly Time { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public int ClientId { get; set; }
    [Required] public Client Client { get; set; } = null!;
    public int ChefId { get; set; }
    [Required] public Chef Chef { get; set; } = null!;
    public ICollection<OrderProduct> Products { get; set; } = new List<OrderProduct>();
}

public enum OrderStatus { Pending = 0, Launched = 1, Cancelled = 2 }
