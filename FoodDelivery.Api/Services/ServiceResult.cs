namespace FoodDelivery.Api.Services;

public sealed record ServiceResult<T>(T? Value, string? Error = null, bool NotFound = false, bool Conflict = false)
    where T : class
{
    public bool Succeeded => Value is not null && Error is null && !NotFound;

    public static ServiceResult<T> Success(T value) => new(value);
    public static ServiceResult<T> Invalid(string error) => new(null, error);
    public static ServiceResult<T> Missing() => new(null, NotFound: true);
    public static ServiceResult<T> Conflicting(string error) => new(null, error, Conflict: true);
}
