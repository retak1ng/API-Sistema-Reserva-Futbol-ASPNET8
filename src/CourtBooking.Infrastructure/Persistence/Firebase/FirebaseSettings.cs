namespace CourtBooking.Infrastructure.Persistence.Firebase;

public sealed class FirebaseSettings
{
    public const string SectionName = "Firebase";

    public string CredentialsPath { get; init; } = string.Empty;
    public string ProjectId { get; init; } = string.Empty;
}