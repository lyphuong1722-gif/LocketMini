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

    // Chỉ chứa các dòng Friend mà UserId == chính user này
    // (lời mời/đã gửi bởi user này, hoặc dòng "đã chấp nhận" thuộc phía user này).
    private readonly List<Friend> _friends = new();

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

    // ── Behaviour: Profile / Post / Password (không đổi) ──────────────────

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

    public void ChangePassword(Password newPassword)
    {
        Password = newPassword;
        RaiseDomainEvent(new UserPasswordChangedEvent(UserId));
    }

    // ── Behaviour: Friend request workflow ─────────────────────────────────
    //
    // LƯU Ý QUAN TRỌNG: các phương thức dưới đây chỉ thao tác trên _friends
    // của CHÍNH aggregate này (những dòng có UserId == this.UserId). Vì một
    // lời mời/kết bạn liên quan tới HAI user, tầng Application chịu trách
    // nhiệm load cả hai aggregate (qua IUserRepository.GetWithFriendsAsync)
    // và gọi đúng phương thức trên đúng aggregate tương ứng.

    /// <summary>Gửi lời mời kết bạn tới user khác (tạo dòng Pending thuộc về mình).</summary>
    public Friend SendFriendRequest(int targetUserId)
    {
        if (targetUserId == UserId)
            throw new DomainException("Không thể gửi lời mời kết bạn cho chính mình.");

        var existing = _friends.FirstOrDefault(f => f.FriendId == targetUserId);
        if (existing is not null)
        {
            throw existing.Status == FriendStatus.Accepted
                ? new DomainException("Hai bạn đã là bạn bè.")
                : new DomainException("Lời mời kết bạn đã được gửi trước đó.");
        }

        var request = Friend.Create(UserId, targetUserId);
        _friends.Add(request);
        RaiseDomainEvent(new FriendRequestSentEvent(UserId, targetUserId));
        return request;
    }

    /// <summary>
    /// Gọi trên aggregate của NGƯỜI GỬI: đánh dấu lời mời (mà mình đã gửi cho
    /// <paramref name="accepterId"/>) chuyển sang trạng thái Accepted.
    /// </summary>
    public void MarkRequestAccepted(int accepterId)
    {
        var request = _friends.FirstOrDefault(f => f.FriendId == accepterId && f.Status == FriendStatus.Pending)
            ?? throw new DomainException("Không tìm thấy lời mời kết bạn.");

        request.Accept();
        RaiseDomainEvent(new FriendRequestAcceptedEvent(UserId, accepterId));
    }

    /// <summary>
    /// Gọi trên aggregate của NGƯỜI CHẤP NHẬN: tạo dòng bạn bè đối xứng
    /// (Accepted) trỏ về phía người đã gửi lời mời.
    /// </summary>
    public void AddAcceptedFriend(int requesterId)
    {
        if (_friends.Any(f => f.FriendId == requesterId))
            throw new DomainException("Đã là bạn bè hoặc quan hệ đã tồn tại.");

        var accepted = Friend.CreateAccepted(UserId, requesterId);
        _friends.Add(accepted);
    }

    /// <summary>Gọi trên aggregate của NGƯỜI GỬI: hủy lời mời đã gửi (chưa được chấp nhận).</summary>
    public void CancelSentRequest(int targetUserId)
    {
        var request = FindPendingRequestTo(targetUserId);
        _friends.Remove(request);
        RaiseDomainEvent(new FriendRequestCancelledEvent(UserId, targetUserId));
    }

    /// <summary>
    /// Gọi trên aggregate của NGƯỜI GỬI: từ chối lời mời (do người nhận từ chối).
    /// Về mặt dữ liệu giống <see cref="CancelSentRequest"/> nhưng khác ngữ nghĩa/sự kiện.
    /// </summary>
    public void DeclineOutgoingRequest(int targetUserId)
    {
        var request = FindPendingRequestTo(targetUserId);
        _friends.Remove(request);
        RaiseDomainEvent(new FriendRequestDeclinedEvent(UserId, targetUserId));
    }

    /// <summary>Hủy kết bạn: xóa dòng Accepted thuộc về mình trỏ tới người kia.</summary>
    public void RemoveFriend(int otherUserId)
    {
        var friendship = _friends.FirstOrDefault(f => f.FriendId == otherUserId && f.Status == FriendStatus.Accepted)
            ?? throw new DomainException($"Không tìm thấy mối quan hệ bạn bè với người dùng {otherUserId}.");

        _friends.Remove(friendship);
        RaiseDomainEvent(new FriendRemovedEvent(UserId, otherUserId));
    }

    private Friend FindPendingRequestTo(int targetUserId)
        => _friends.FirstOrDefault(f => f.FriendId == targetUserId && f.Status == FriendStatus.Pending)
           ?? throw new DomainException("Không tìm thấy lời mời kết bạn.");
}