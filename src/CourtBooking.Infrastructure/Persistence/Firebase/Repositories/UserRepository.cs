using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Domain.Entities;
using CourtBooking.Domain.Enums;
using Google.Cloud.Firestore;

namespace CourtBooking.Infrastructure.Persistence.Firebase.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly FirebaseContext _context;
    private CollectionReference Collection =>
        _context.Database.Collection(FirebaseContext.Collections.Users);

    public UserRepository(FirebaseContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var doc = await Collection.Document(id).GetSnapshotAsync(cancellationToken);

        if (!doc.Exists) return null;

        return MapToUser(doc);
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var query = await Collection
            .WhereEqualTo("Email", email.ToLowerInvariant())
            .Limit(1)
            .GetSnapshotAsync(cancellationToken);

        var doc = query.Documents.FirstOrDefault();

        if (doc is null) return null;

        return MapToUser(doc);
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var query = await Collection
            .WhereEqualTo("Email", email.ToLowerInvariant())
            .Limit(1)
            .GetSnapshotAsync(cancellationToken);

        return query.Documents.Any();
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>
        {
            { "Id", user.Id },
            { "Name", user.Name },
            { "Email", user.Email },
            { "PasswordHash", user.PasswordHash },
            { "Role", user.Role.ToString() },
            { "CreatedAt", user.CreatedAt }
        };

        await Collection.Document(user.Id).SetAsync(data, null, cancellationToken);
    }

    // Mapeo manual: Firestore → Entidad de dominio
    // No usamos ORM — el dominio permanece limpio
    private static User MapToUser(DocumentSnapshot doc)
    {
        var roleString = doc.GetValue<string>("Role");
        var role = Enum.Parse<UserRole>(roleString);

        return User.Create(
            name: doc.GetValue<string>("Name"),
            email: doc.GetValue<string>("Email"),
            passwordHash: doc.GetValue<string>("PasswordHash"),
            role: role
        ).Value;
    }
}