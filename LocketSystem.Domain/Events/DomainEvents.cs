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

// ── Friend events ─────────────────────────────────────────────────────────────

public sealed record FriendAddedEvent(int UserId, int FriendId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record FriendRemovedEvent(int UserId, int FriendId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
