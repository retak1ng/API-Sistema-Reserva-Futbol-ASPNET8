using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Domain.Entities;
using Google.Cloud.Firestore;

namespace CourtBooking.Infrastructure.Persistence.Firebase.Repositories;

public sealed class CourtRepository : ICourtRepository
{
    private readonly FirebaseContext _context;
    private CollectionReference Collection =>
        _context.Database.Collection(FirebaseContext.Collections.Courts);

    public CourtRepository(FirebaseContext context)
    {
        _context = context;
    }

    public async Task<Court?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var doc = await Collection.Document(id).GetSnapshotAsync(cancellationToken);

        if (!doc.Exists) return null;

        return MapToCourt(doc);
    }

    public async Task<IReadOnlyList<Court>> GetAllActiveAsync(
        CancellationToken cancellationToken = default)
    {
        var query = await Collection
            .WhereEqualTo("IsActive", true)
            .GetSnapshotAsync(cancellationToken);

        return query.Documents
            .Select(MapToCourt)
            .ToList()
            .AsReadOnly();
    }

    public async Task AddAsync(
        Court court,
        CancellationToken cancellationToken = default)
    {
        var data = MapToFirestore(court);
        await Collection.Document(court.Id).SetAsync(data, null, cancellationToken);
    }

    public async Task UpdateAsync(
        Court court,
        CancellationToken cancellationToken = default)
    {
        var data = MapToFirestore(court);
        await Collection.Document(court.Id).SetAsync(data, null, cancellationToken);
    }

    private static Dictionary<string, object> MapToFirestore(Court court) =>
        new()
        {
            { "Id", court.Id },
            { "Name", court.Name },
            { "Description", court.Description },
            { "PricePerHour", court.PricePerHour.ToString() },
            { "IsActive", court.IsActive },
            { "CreatedAt", court.CreatedAt }
        };

    private static Court MapToCourt(DocumentSnapshot doc)
    {
        var priceStr = doc.GetValue<string>("PricePerHour");
        var price = decimal.Parse(priceStr);

        return Court.Create(
            name: doc.GetValue<string>("Name"),
            description: doc.GetValue<string>("Description"),
            pricePerHour: price
        ).Value;
    }
}