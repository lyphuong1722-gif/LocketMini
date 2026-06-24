using LocketMini.Domain.Common;
using LocketMini.Domain.Events;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.ValueObjects;

namespace LocketMini.Domain.Entities;

public sealed class User : BaseEntity
{
    // ── Private backing fields ────────────────────────────────────────────
    private readonly List<Post> _posts = new();
    private readonly List<Comment> _comments = new();
    private readonly List<Like> _likes = new();
    private readonly List<Friend> _friends = new();  // rows where this user is the "owner"

    // ── Identity ──────────────────────────────────────────────────────────
    public int UserId { get; private set; }
    public Username Username { get; private set; } = null!;
    public Password Password { get; private set; } = null!;

    // ── Navigation ────────────────────────────────────────────────────────
    public Profile? Profile { get; private set; }
    public IReadOnlyCollection<Post> Posts => _posts.AsReadOnly();
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();
    public IReadOnlyCollection<Like> Likes => _likes.AsReadOnly();
    public IReadOnlyCollection<Friend> Friends => _friends.AsReadOnly();

    // ── EF Core constructor ───────────────────────────────────────────────
    private User() { }

    // ── Factory method ────────────────────────────────────────────────────
    public static User Create(string username, string hashedPassword)
    {
        var user = new User
        {
            Username = Username.Create(username),
            Password = Password.Create(hashedPassword)
        };

        user.RaiseDomainEvent(new UserCreatedEvent(user.UserId, user.Username.Value));
        return user;
    }

    // ── Behaviour ─────────────────────────────────────────────────────────

    public void SetProfile(string? fullName, string? bio)
    {
        if (Profile is null)
            Profile = Profile.Create(UserId, fullName, bio);
        else
            Profile.Update(fullName, bio);
    }

    public Post AddPost(string? caption, string? imageUrl)
    {
        var post = Post.Create(UserId, caption, imageUrl);
        _posts.Add(post);
        RaiseDomainEvent(new PostCreatedEvent(UserId, post.PostId));
        return post;
    }

    public void AddFriend(User friendUser)
    {
        if (friendUser.UserId == UserId)
            throw new DomainException("Không thể kết bạn với chính mình.");

        bool alreadyFriends = _friends.Any(f => f.FriendId == friendUser.UserId);
        if (alreadyFriends)
            throw new DomainException($"Đã là bạn bè với người dùng {friendUser.UserId}.");

        var friendship = Friend.Create(UserId, friendUser.UserId);
        _friends.Add(friendship);
        RaiseDomainEvent(new FriendAddedEvent(UserId, friendUser.UserId));
    }

    public void RemoveFriend(int friendUserId)
    {
        var friendship = _friends.FirstOrDefault(f => f.FriendId == friendUserId)
            ?? throw new DomainException($"Không tìm thấy mối quan hệ bạn bè với người dùng {friendUserId}.");

        _friends.Remove(friendship);
    }

    public void ChangePassword(Password newPassword)
    {
        Password = newPassword;
        RaiseDomainEvent(new UserPasswordChangedEvent(UserId));
    }
}
