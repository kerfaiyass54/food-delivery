using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Contracts.Commerce;

public sealed record CreateOrderRequest(
    [property: Required, MinLength(1)] IReadOnlyList<OrderLineRequest> Products,
    TimeOnly Time);
