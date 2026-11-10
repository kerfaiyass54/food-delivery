namespace FoodDelivery.Api.Entities;

public class RestaurantOffDay
{
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    public int DayId { get; set; }
    public Day Day { get; set; } = null!;
}
