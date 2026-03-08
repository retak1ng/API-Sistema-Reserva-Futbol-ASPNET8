using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Domain.Entities;
using CourtBooking.Domain.Enums;
using Google.Cloud.Firestore;

namespace CourtBooking.Infrastructure.Persistence.Firebase.Repositories;

public sealed class ReservationRepository : IReservationRepository
{
    private readonly FirebaseContext _context;
    private CollectionReference Collection =>
        _context.Database.Collection(FirebaseContext.Collections.Reservations);

    public ReservationRepository(FirebaseContext context)
    {
        _context = context;
    }

    public async Task<Reservation?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var doc = await Collection.Document(id).GetSnapshotAsync(cancellationToken);

        if (!doc.Exists) return null;

        return MapToReservation(doc);
    }

    public async Task<IReadOnlyList<Reservation>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var query = await Collection
            .WhereEqualTo("UserId", userId)
            .GetSnapshotAsync(cancellationToken);

        return query.Documents
            .Select(MapToReservation)
            .ToList()
            .AsReadOnly();
    }

    public async Task<IReadOnlyList<Reservation>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var query = await Collection.GetSnapshotAsync(cancellationToken);

        return query.Documents
            .Select(MapToReservation)
            .ToList()
            .AsReadOnly();
    }

    public async Task AddAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default)
    {
        var data = MapToFirestore(reservation);
        await Collection.Document(reservation.Id).SetAsync(data, null, cancellationToken);
    }

    public async Task UpdateAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default)
    {
        var data = MapToFirestore(reservation);
        await Collection.Document(reservation.Id).SetAsync(data, null, cancellationToken);
    }

    private static Dictionary<string, object> MapToFirestore(Reservation reservation) =>
        new()
        {
            { "Id", reservation.Id },
            { "UserId", reservation.UserId },
            { "CourtId", reservation.CourtId },
            { "AvailabilityId", reservation.AvailabilityId },
            { "Date", reservation.Date.ToString("yyyy-MM-dd") },
            { "StartTime", reservation.StartTime.ToString("HH:mm") },
            { "EndTime", reservation.EndTime.ToString("HH:mm") },
            { "TotalPrice", reservation.TotalPrice.ToString() },
            { "Status", reservation.Status.ToString() },
            { "CreatedAt", reservation.CreatedAt }
        };

    private static Reservation MapToReservation(DocumentSnapshot doc)
    {
        var date = DateOnly.Parse(doc.GetValue<string>("Date"));
        var startTime = TimeOnly.Parse(doc.GetValue<string>("StartTime"));
        var endTime = TimeOnly.Parse(doc.GetValue<string>("EndTime"));
        var totalPrice = decimal.Parse(doc.GetValue<string>("TotalPrice"));
        var status = Enum.Parse<ReservationStatus>(doc.GetValue<string>("Status"));

        return Reservation.Create(
            userId: doc.GetValue<string>("UserId"),
            courtId: doc.GetValue<string>("CourtId"),
            availabilityId: doc.GetValue<string>("AvailabilityId"),
            date: date,
            startTime: startTime,
            endTime: endTime,
            totalPrice: totalPrice
        ).Value;
    }
}