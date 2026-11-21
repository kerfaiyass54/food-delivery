using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Contracts.Commerce;

public sealed record PurchaseRequest(
    [property: Range(1, int.MaxValue)] int ProductId,
    [property: Range(1, int.MaxValue)] int Quantity,
    [property: Range(typeof(decimal), "0.01", "79228162514264337593543950335")] decimal Price);
