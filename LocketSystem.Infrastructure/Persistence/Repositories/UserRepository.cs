using LocketMini.Domain.Entities;
using LocketMini.Domain.Interfaces.Repositories;
using LocketMini.Infrastructure.Common;
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

    /// <summary>
    /// Tìm kiếm theo Username hoặc FullName, không phân biệt hoa/thường và có dấu/không dấu.
    /// So khớp được thực hiện ở tầng ứng dụng vì SQL Server LIKE mặc định
    /// không tự động bỏ dấu tiếng Việt.
    /// </summary>
    public async Task<IReadOnlyList<User>> SearchAsync(string keyword, CancellationToken ct = default)
    {
        var normalizedKeyword = TextNormalizer.NormalizeForSearch(keyword);

        var allUsers = await Set
            .Include(u => u.Profile)
            .ToListAsync(ct);

        return allUsers
            .Where(u =>
                TextNormalizer.NormalizeForSearch(u.Username.Value).Contains(normalizedKeyword) ||
                (u.Profile?.FullName is not null &&
                 TextNormalizer.NormalizeForSearch(u.Profile.FullName).Contains(normalizedKeyword)))
            .OrderBy(u => u.Username.Value)
            .ToList();
    }
}