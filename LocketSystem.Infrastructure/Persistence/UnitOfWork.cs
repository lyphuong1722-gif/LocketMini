using LocketMini.Domain.Interfaces.Repositories;
using LocketMini.Infrastructure.Persistence.Repositories;

namespace LocketMini.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    // Lazy init để tránh tạo repository không cần thiết
    private IUserRepository? _users;
    private IPostRepository? _posts;
    private ICommentRepository? _comments;
    private ILikeRepository? _likes;
    private IFriendRepository? _friends;

    public UnitOfWork(AppDbContext db) => _db = db;

    public IUserRepository Users => _users ??= new UserRepository(_db);
    public IPostRepository Posts => _posts ??= new PostRepository(_db);
    public ICommentRepository Comments => _comments ??= new CommentRepository(_db);
    public ILikeRepository Likes => _likes ??= new LikeRepository(_db);
    public IFriendRepository Friends => _friends ??= new FriendRepository(_db);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
