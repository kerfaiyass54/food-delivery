using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Contracts.Commerce;

public sealed record PurchaseDto(
    int Id,
    int ClientId,
    int ProductId,
    DateTimeOffset Date,
    decimal Price,
    int Quantity)
{
    public static PurchaseDto From(Purchase purchase) =>
        new(purchase.Id, purchase.ClientId, purchase.ProductId, purchase.Date,
            purchase.Price, purchase.Quantity);
}
