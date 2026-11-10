using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Entities;

public class Day
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string Name { get; set; } = string.Empty;

    public ICollection<RestaurantOperatingDay> OperatingRestaurants { get; set; } = new List<RestaurantOperatingDay>();

    public ICollection<RestaurantOffDay> RestaurantsOff { get; set; } = new List<RestaurantOffDay>();
}
