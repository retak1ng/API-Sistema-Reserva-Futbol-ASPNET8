using CourtBooking.Domain.Entities;

namespace CourtBooking.Application.Abstractions.Repositories;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reservation>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);
}