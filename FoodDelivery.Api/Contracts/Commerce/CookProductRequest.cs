using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Contracts.Commerce;

public sealed record CookProductRequest(
    [property: Required, StringLength(150)] string Name,
    [property: Required, StringLength(100)] string Type,
    [property: Range(1, int.MaxValue)] int Quantity);
