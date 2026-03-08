using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Domain.Entities;
using Google.Cloud.Firestore;

namespace CourtBooking.Infrastructure.Persistence.Firebase.Repositories;

public sealed class AvailabilityRepository : IAvailabilityRepository
{
    private readonly FirebaseContext _context;
    private CollectionReference Collection =>
        _context.Database.Collection(FirebaseContext.Collections.Availabilities);

    public AvailabilityRepository(FirebaseContext context)
    {
        _context = context;
    }

    public async Task<Availability?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var doc = await Collection.Document(id).GetSnapshotAsync(cancellationToken);

        if (!doc.Exists) return null;

        return MapToAvailability(doc);
    }

    public async Task<IReadOnlyList<Availability>> GetByCourtAndDateAsync(
        string courtId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var dateString = date.ToString("yyyy-MM-dd");

        var query = await Collection
            .WhereEqualTo("CourtId", courtId)
            .WhereEqualTo("Date", dateString)
            .GetSnapshotAsync(cancellationToken);

        return query.Documents
            .Select(MapToAvailability)
            .ToList()
            .AsReadOnly();
    }

    public async Task AddAsync(
        Availability availability,
        CancellationToken cancellationToken = default)
    {
        var data = MapToFirestore(availability);
        await Collection.Document(availability.Id).SetAsync(data, null, cancellationToken);
    }

    public async Task UpdateAsync(
        Availability availability,
        CancellationToken cancellationToken = default)
    {
        var data = MapToFirestore(availability);
        await Collection.Document(availability.Id).SetAsync(data, null, cancellationToken);
    }

    private static Dictionary<string, object> MapToFirestore(Availability availability) =>
        new()
        {
            { "Id", availability.Id },
            { "CourtId", availability.CourtId },
            { "Date", availability.Date.ToString("yyyy-MM-dd") },
            { "StartTime", availability.StartTime.ToString("HH:mm") },
            { "EndTime", availability.EndTime.ToString("HH:mm") },
            { "IsBooked", availability.IsBooked },
            { "CreatedAt", availability.CreatedAt }
        };

    private static Availability MapToAvailability(DocumentSnapshot doc)
    {
        var date = DateOnly.Parse(doc.GetValue<string>("Date"));
        var startTime = TimeOnly.Parse(doc.GetValue<string>("StartTime"));
        var endTime = TimeOnly.Parse(doc.GetValue<string>("EndTime"));

        return Availability.Create(
            courtId: doc.GetValue<string>("CourtId"),
            date: date,
            startTime: startTime,
            endTime: endTime
        ).Value;
    }
}