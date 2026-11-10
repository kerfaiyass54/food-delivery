namespace FoodDelivery.Api.Entities;

// Explicit join entity keeps operating days separate from days off.
public class RestaurantOperatingDay
{
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    public int DayId { get; set; }
    public Day Day { get; set; } = null!;
}
