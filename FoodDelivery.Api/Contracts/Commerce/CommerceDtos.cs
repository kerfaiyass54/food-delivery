using System.ComponentModel.DataAnnotations;
using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Contracts.Commerce;

public sealed record CreateClientRequest([property: Required, StringLength(150)] string Name,
    [property: Required, StringLength(100)] string City, [property: Required, StringLength(100)] string Nationality,
    [property: Range(typeof(decimal), "0", "79228162514264337593543950335")] decimal Budget,
    [property: Range(typeof(decimal), "0", "79228162514264337593543950335")] decimal Weight,
    ClientStatus Status = ClientStatus.Normal);
public sealed record ClientDto(int Id, string Name, string City, string Nationality, decimal Budget, decimal Weight,
    string Status)
{ public static ClientDto From(Client x) => new(x.Id, x.Name, x.City, x.Nationality, x.Budget, x.Weight, x.Status.ToString()); }

public sealed record CookProductRequest([property: Required, StringLength(150)] string Name,
    [property: Required, StringLength(100)] string Type, [property: Range(1, int.MaxValue)] int Quantity);
public sealed record ProductDto(int Id, string Name, string Type, int Stock, DateTimeOffset CookDate, int ChefId)
{ public static ProductDto From(Product x) => new(x.Id, x.Name, x.Type, x.Stock, x.CookDate, x.ChefId); }

public sealed record PurchaseRequest([property: Range(1, int.MaxValue)] int ProductId,
    [property: Range(1, int.MaxValue)] int Quantity,
    [property: Range(typeof(decimal), "0.01", "79228162514264337593543950335")] decimal Price);
public sealed record PurchaseDto(int Id, int ClientId, int ProductId, DateTimeOffset Date, decimal Price, int Quantity)
{ public static PurchaseDto From(Purchase x) => new(x.Id, x.ClientId, x.ProductId, x.Date, x.Price, x.Quantity); }

public sealed record OrderLineRequest([property: Range(1, int.MaxValue)] int ProductId,
    [property: Range(1, int.MaxValue)] int Quantity);
public sealed record CreateOrderRequest([property: Required, MinLength(1)] IReadOnlyList<OrderLineRequest> Products,
    TimeOnly Time);
public sealed record OrderLineDto(int ProductId, string ProductName, int Quantity);
public sealed record OrderDto(int Id, int ClientId, int ChefId, DateTimeOffset Date, TimeOnly Time, string Status,
    IReadOnlyList<OrderLineDto> Products)
{
    public static OrderDto From(FoodOrder x) => new(x.Id, x.ClientId, x.ChefId, x.Date, x.Time, x.Status.ToString(),
        x.Products.Select(p => new OrderLineDto(p.ProductId, p.Product.Name, p.Quantity)).ToArray());
}
