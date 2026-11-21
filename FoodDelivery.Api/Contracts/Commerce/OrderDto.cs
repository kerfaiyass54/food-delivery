using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Contracts.Commerce;

public sealed record OrderDto(
    int Id,
    int ClientId,
    int ChefId,
    DateTimeOffset Date,
    TimeOnly Time,
    string Status,
    IReadOnlyList<OrderLineDto> Products)
{
    public static OrderDto From(FoodOrder order) =>
        new(order.Id, order.ClientId, order.ChefId, order.Date, order.Time,
            order.Status.ToString(),
            order.Products.Select(p =>
                new OrderLineDto(p.ProductId, p.Product.Name, p.Quantity)).ToArray());
}
