using LocketMini.Domain.Entities;

namespace LocketMini.Domain.Interfaces.Repositories;

// ── Generic base ──────────────────────────────────────────────────────────────

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}

// ── User ──────────────────────────────────────────────────────────────────────

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<bool> ExistsAsync(string username, CancellationToken ct = default);

    /// <summary>Lấy user kèm Profile.</summary>
    Task<User?> GetWithProfileAsync(int userId, CancellationToken ct = default);

    /// <summary>Lấy user kèm danh sách bạn bè / lời mời (Friends) do chính user này sở hữu (UserId == userId).</summary>
    Task<User?> GetWithFriendsAsync(int userId, CancellationToken ct = default);

    /// <summary>Tìm kiếm user theo username hoặc FullName.</summary>
    Task<IReadOnlyList<User>> SearchAsync(string keyword, CancellationToken ct = default);
}

// ── Post ──────────────────────────────────────────────────────────────────────

public interface IPostRepository : IRepository<Post>
{
    /// <summary>Lấy post kèm Likes và Comments.</summary>
    Task<Post?> GetWithDetailsAsync(int postId, CancellationToken ct = default);

    /// <summary>Feed: tất cả bài viết của danh sách user (bạn bè).</summary>
    Task<IReadOnlyList<Post>> GetFeedAsync(
        IEnumerable<int> friendIds,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<IReadOnlyList<Post>> GetByUserAsync(
        int userId,
        CancellationToken ct = default);

    /// <summary>
    /// Đếm tổng số bài viết của danh sách tác giả (dùng để phân trang Feed chính xác,
    /// KHÔNG áp dụng Skip/Take).
    /// </summary>
    Task<int> CountFeedAsync(
        IEnumerable<int> authorIds,
        CancellationToken ct = default);
}

// ── Comment ───────────────────────────────────────────────────────────────────

public interface ICommentRepository : IRepository<Comment>
{
    Task<IReadOnlyList<Comment>> GetByPostAsync(int postId, CancellationToken ct = default);
}

// ── Like ──────────────────────────────────────────────────────────────────────

public interface ILikeRepository : IRepository<Like>
{
    Task<Like?> GetAsync(int userId, int postId, CancellationToken ct = default);
    Task<int> CountByPostAsync(int postId, CancellationToken ct = default);
}

// ── Friend ────────────────────────────────────────────────────────────────────

public interface IFriendRepository
{
    /// <summary>Hai người có đang là bạn bè (Accepted) hay không.</summary>
    Task<bool> AreFriendsAsync(int userId, int friendId, CancellationToken ct = default);

    /// <summary>Danh sách UserId bạn bè đã Accepted của userId.</summary>
    Task<IReadOnlyList<int>> GetFriendIdsAsync(int userId, CancellationToken ct = default);

    /// <summary>Lấy đúng 1 dòng directed (UserId=userId, FriendId=friendId) — dùng để kiểm tra trạng thái quan hệ.</summary>
    Task<Friend?> GetDirectedAsync(int userId, int friendId, CancellationToken ct = default);

    /// <summary>Các lời mời ĐANG CHỜ mà userId là người NHẬN (người khác gửi cho userId).</summary>
    Task<IReadOnlyList<Friend>> GetIncomingRequestsAsync(int userId, CancellationToken ct = default);

    /// <summary>Các lời mời ĐANG CHỜ mà userId là người GỬI (userId gửi cho người khác).</summary>
    Task<IReadOnlyList<Friend>> GetOutgoingRequestsAsync(int userId, CancellationToken ct = default);
}

// ── Unit of Work ──────────────────────────────────────────────────────────────

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IPostRepository Posts { get; }
    ICommentRepository Comments { get; }
    ILikeRepository Likes { get; }
    IFriendRepository Friends { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}