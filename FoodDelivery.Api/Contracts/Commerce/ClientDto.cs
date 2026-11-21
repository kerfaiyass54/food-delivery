using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Contracts.Commerce;

public sealed record ClientDto(
    int Id,
    string Name,
    string City,
    string Nationality,
    decimal Budget,
    decimal Weight,
    string Status)
{
    public static ClientDto From(Client client) =>
        new(client.Id, client.Name, client.City, client.Nationality, client.Budget,
            client.Weight, client.Status.ToString());
}
