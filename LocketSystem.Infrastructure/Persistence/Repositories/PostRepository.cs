using LocketMini.Domain.Entities;
using LocketMini.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LocketMini.Infrastructure.Persistence.Repositories;

public sealed class PostRepository : BaseRepository<Post>, IPostRepository
{
    public PostRepository(AppDbContext db) : base(db) { }

    public async Task<Post?> GetWithDetailsAsync(int postId, CancellationToken ct = default)
        => await Set
            .Include(p => p.Owner)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Likes)
                .ThenInclude(l => l.User)
            .FirstOrDefaultAsync(p => p.PostId == postId, ct);

    public async Task<IReadOnlyList<Post>> GetFeedAsync(
        IEnumerable<int> authorIds,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var ids = authorIds.ToList();

        return await Set
            .Where(p => ids.Contains(p.UserId))
            .Include(p => p.Owner)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Post>> GetByUserAsync(int userId, CancellationToken ct = default)
        => await Set
            .Where(p => p.UserId == userId)
            .Include(p => p.Owner)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);
}
