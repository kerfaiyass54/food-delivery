using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Api.Contracts.Commerce;

public sealed record OrderLineRequest(
    [property: Range(1, int.MaxValue)] int ProductId,
    [property: Range(1, int.MaxValue)] int Quantity);
