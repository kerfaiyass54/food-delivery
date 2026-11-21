using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Contracts.Commerce;

public sealed record ProductDto(int Id, string Name, string Type, int Stock,
    DateTimeOffset CookDate, int ChefId)
{
    public static ProductDto From(Product product) =>
        new(product.Id, product.Name, product.Type, product.Stock,
            product.CookDate, product.ChefId);
}
