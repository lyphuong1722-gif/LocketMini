using LocketMini.Domain.Entities;
using LocketMini.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LocketMini.Infrastructure.Persistence.Repositories;

// ── CommentRepository ─────────────────────────────────────────────────────────

public sealed class CommentRepository : BaseRepository<Comment>, ICommentRepository
{
    public CommentRepository(AppDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Comment>> GetByPostAsync(int postId, CancellationToken ct = default)
        => await Set
            .Where(c => c.PostId == postId)
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(ct);
}

// ── LikeRepository ────────────────────────────────────────────────────────────

public sealed class LikeRepository : BaseRepository<Like>, ILikeRepository
{
    public LikeRepository(AppDbContext db) : base(db) { }

    public async Task<Like?> GetAsync(int userId, int postId, CancellationToken ct = default)
        => await Set
            .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId, ct);

    public async Task<int> CountByPostAsync(int postId, CancellationToken ct = default)
        => await Set.CountAsync(l => l.PostId == postId, ct);
}

// ── FriendRepository ──────────────────────────────────────────────────────────

public sealed class FriendRepository : IFriendRepository
{
    private readonly AppDbContext _db;

    public FriendRepository(AppDbContext db) => _db = db;

    public async Task<bool> AreFriendsAsync(int userId, int friendId, CancellationToken ct = default)
        => await _db.Friends
            .AnyAsync(f => f.UserId == userId && f.FriendId == friendId && f.Status == FriendStatus.Accepted, ct);

    public async Task<IReadOnlyList<int>> GetFriendIdsAsync(int userId, CancellationToken ct = default)
        => await _db.Friends
            .Where(f => f.UserId == userId && f.Status == FriendStatus.Accepted)
            .Select(f => f.FriendId)
            .ToListAsync(ct);

    public async Task<Friend?> GetDirectedAsync(int userId, int friendId, CancellationToken ct = default)
        => await _db.Friends
            .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId, ct);

    public async Task<IReadOnlyList<Friend>> GetIncomingRequestsAsync(int userId, CancellationToken ct = default)
        => await _db.Friends
            .Where(f => f.FriendId == userId && f.Status == FriendStatus.Pending)
            .Include(f => f.User) // người gửi lời mời
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Friend>> GetOutgoingRequestsAsync(int userId, CancellationToken ct = default)
        => await _db.Friends
            .Where(f => f.UserId == userId && f.Status == FriendStatus.Pending)
            .Include(f => f.FriendUser) // người nhận lời mời
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);
}