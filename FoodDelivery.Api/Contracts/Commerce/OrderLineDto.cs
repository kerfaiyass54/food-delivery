namespace FoodDelivery.Api.Contracts.Commerce;

public sealed record OrderLineDto(int ProductId, string ProductName, int Quantity);
