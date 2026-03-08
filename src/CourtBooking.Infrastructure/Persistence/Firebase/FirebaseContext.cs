using Google.Cloud.Firestore;
using Microsoft.Extensions.Options;

namespace CourtBooking.Infrastructure.Persistence.Firebase;

public sealed class FirebaseContext
{
    public FirestoreDb Database { get; }

    // Nombres de colecciones como constantes — sin magic strings
    public static class Collections
    {
        public const string Users = "users";
        public const string Courts = "courts";
        public const string Availabilities = "availabilities";
        public const string Reservations = "reservations";
    }

    public FirebaseContext(IOptions<FirebaseSettings> settings)
    {
        var credentialsPath = settings.Value.CredentialsPath;
        var projectId = settings.Value.ProjectId;

        // Setear variable de entorno para que Google lo tome automáticamente
        Environment.SetEnvironmentVariable(
            "GOOGLE_APPLICATION_CREDENTIALS",
            credentialsPath);

        Database = FirestoreDb.Create(projectId);
    }
}