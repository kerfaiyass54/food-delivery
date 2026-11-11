namespace FoodDelivery.Api.Data.Candidatures;

public sealed record CandidatureCreationResult(CandidatureCreationStatus Status, int? CandidatureId = null);
