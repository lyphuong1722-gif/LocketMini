using LocketMini.Domain.Entities;
using LocketMini.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LocketMini.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db) { }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => await Set
            .FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<bool> ExistsAsync(string username, CancellationToken ct = default)
        => await Set
            .AnyAsync(u => u.Username == username, ct);

    public async Task<User?> GetWithProfileAsync(int userId, CancellationToken ct = default)
        => await Set
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.UserId == userId, ct);

    public async Task<User?> GetWithFriendsAsync(int userId, CancellationToken ct = default)
        => await Set
            .Include(u => u.Friends)
            .FirstOrDefaultAsync(u => u.UserId == userId, ct);

    public async Task<IReadOnlyList<User>> SearchAsync(string keyword, CancellationToken ct = default)
    {
        var lower = keyword.ToLower();

        return await Set
            .Include(u => u.Profile)
            .Where(u =>
                u.Username.Value.Contains(lower) ||
                (u.Profile != null && u.Profile.FullName != null &&
                 u.Profile.FullName.ToLower().Contains(lower)))
            .OrderBy(u => u.Username.Value)
            .ToListAsync(ct);
    }
}
