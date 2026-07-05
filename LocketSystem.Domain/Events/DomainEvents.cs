using LocketMini.Domain.Common;

namespace LocketMini.Domain.Events;

// ── User events ───────────────────────────────────────────────────────────────

public sealed record UserCreatedEvent(int UserId, string Username) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record UserPasswordChangedEvent(int UserId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ── Post events ───────────────────────────────────────────────────────────────

public sealed record PostCreatedEvent(int OwnerId, int PostId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record PostLikedEvent(int PostId, int LikerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record PostUnlikedEvent(int PostId, int UnlikerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ── Comment events ────────────────────────────────────────────────────────────

public sealed record CommentAddedEvent(int PostId, int CommenterId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// ── Friend request workflow events ────────────────────────────────────────────

/// <summary>RequesterId đã gửi lời mời kết bạn cho TargetId.</summary>
public sealed record FriendRequestSentEvent(int RequesterId, int TargetId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>AccepterId đã chấp nhận lời mời kết bạn từ RequesterId.</summary>
public sealed record FriendRequestAcceptedEvent(int RequesterId, int AccepterId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>TargetId đã từ chối lời mời kết bạn từ RequesterId.</summary>
public sealed record FriendRequestDeclinedEvent(int RequesterId, int TargetId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>RequesterId đã hủy lời mời kết bạn (chưa được phản hồi) gửi tới TargetId.</summary>
public sealed record FriendRequestCancelledEvent(int RequesterId, int TargetId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>UserId đã hủy kết bạn với FriendId (đã từng là bạn bè).</summary>
public sealed record FriendRemovedEvent(int UserId, int FriendId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}