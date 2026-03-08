using CourtBooking.Domain.Entities;

namespace CourtBooking.Application.Abstractions.Repositories;

public interface ICourtRepository
{
    Task<Court?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Court>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Court court, CancellationToken cancellationToken = default);
    Task UpdateAsync(Court court, CancellationToken cancellationToken = default);
}