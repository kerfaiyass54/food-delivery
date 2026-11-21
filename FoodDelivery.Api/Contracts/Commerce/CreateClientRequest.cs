using System.ComponentModel.DataAnnotations;
using FoodDelivery.Api.Entities;

namespace FoodDelivery.Api.Contracts.Commerce;

public sealed record CreateClientRequest(
    [property: Required, StringLength(150)] string Name,
    [property: Required, StringLength(100)] string City,
    [property: Required, StringLength(100)] string Nationality,
    [property: Range(typeof(decimal), "0", "79228162514264337593543950335")] decimal Budget,
    [property: Range(typeof(decimal), "0", "79228162514264337593543950335")] decimal Weight,
    ClientStatus Status = ClientStatus.Normal);
