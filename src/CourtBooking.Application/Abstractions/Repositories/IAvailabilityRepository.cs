using CourtBooking.Domain.Entities;

namespace CourtBooking.Application.Abstractions.Repositories;

public interface IAvailabilityRepository
{
    Task<Availability?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Availability>> GetByCourtAndDateAsync(
        string courtId,
        DateOnly date,
        CancellationToken cancellationToken = default);
    Task AddAsync(Availability availability, CancellationToken cancellationToken = default);
    Task UpdateAsync(Availability availability, CancellationToken cancellationToken = default);
}