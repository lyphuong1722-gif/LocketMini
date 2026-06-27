using LocketMini.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocketMini.Application.EventHandlers;

// ── UserCreated ───────────────────────────────────────────────────────────────

public sealed class UserCreatedEventHandler
    : INotificationHandler<DomainEventNotification<UserCreatedEvent>>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;
    public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<UserCreatedEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] Người dùng mới: UserId={UserId}, Username={Username}",
            n.DomainEvent.UserId, n.DomainEvent.Username);

        // TODO: gửi email chào mừng qua IEmailService
        return Task.CompletedTask;
    }
}

// ── PostCreated ───────────────────────────────────────────────────────────────

public sealed class PostCreatedEventHandler
    : INotificationHandler<DomainEventNotification<PostCreatedEvent>>
{
    private readonly ILogger<PostCreatedEventHandler> _logger;
    public PostCreatedEventHandler(ILogger<PostCreatedEventHandler> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<PostCreatedEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] Bài viết mới: PostId={PostId}, OwnerId={OwnerId}",
            n.DomainEvent.PostId, n.DomainEvent.OwnerId);

        // TODO: push notification tới bạn bè qua INotificationService
        return Task.CompletedTask;
    }
}

// ── PostLiked ─────────────────────────────────────────────────────────────────

public sealed class PostLikedEventHandler
    : INotificationHandler<DomainEventNotification<PostLikedEvent>>
{
    private readonly ILogger<PostLikedEventHandler> _logger;
    public PostLikedEventHandler(ILogger<PostLikedEventHandler> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<PostLikedEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] PostId={PostId} được thích bởi UserId={LikerId}",
            n.DomainEvent.PostId, n.DomainEvent.LikerId);

        // TODO: notify chủ bài viết
        return Task.CompletedTask;
    }
}

// ── PostUnliked ───────────────────────────────────────────────────────────────

public sealed class PostUnlikedEventHandler
    : INotificationHandler<DomainEventNotification<PostUnlikedEvent>>
{
    private readonly ILogger<PostUnlikedEventHandler> _logger;
    public PostUnlikedEventHandler(ILogger<PostUnlikedEventHandler> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<PostUnlikedEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] PostId={PostId} bỏ thích bởi UserId={UnlikerId}",
            n.DomainEvent.PostId, n.DomainEvent.UnlikerId);
        return Task.CompletedTask;
    }
}

// ── CommentAdded ──────────────────────────────────────────────────────────────

public sealed class CommentAddedEventHandler
    : INotificationHandler<DomainEventNotification<CommentAddedEvent>>
{
    private readonly ILogger<CommentAddedEventHandler> _logger;
    public CommentAddedEventHandler(ILogger<CommentAddedEventHandler> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<CommentAddedEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] Bình luận mới trên PostId={PostId} bởi UserId={CommenterId}",
            n.DomainEvent.PostId, n.DomainEvent.CommenterId);

        // TODO: notify chủ bài viết
        return Task.CompletedTask;
    }
}

// ── FriendAdded ───────────────────────────────────────────────────────────────

public sealed class FriendAddedEventHandler
    : INotificationHandler<DomainEventNotification<FriendAddedEvent>>
{
    private readonly ILogger<FriendAddedEventHandler> _logger;
    public FriendAddedEventHandler(ILogger<FriendAddedEventHandler> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<FriendAddedEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] UserId={UserId} kết bạn với FriendId={FriendId}",
            n.DomainEvent.UserId, n.DomainEvent.FriendId);

        // TODO: notify người được kết bạn
        return Task.CompletedTask;
    }
}

// ── FriendRemoved ─────────────────────────────────────────────────────────────

public sealed class FriendRemovedEventHandler
    : INotificationHandler<DomainEventNotification<FriendRemovedEvent>>
{
    private readonly ILogger<FriendRemovedEventHandler> _logger;
    public FriendRemovedEventHandler(ILogger<FriendRemovedEventHandler> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<FriendRemovedEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] UserId={UserId} hủy kết bạn với FriendId={FriendId}",
            n.DomainEvent.UserId, n.DomainEvent.FriendId);
        return Task.CompletedTask;
    }
}

// ── UserPasswordChanged ───────────────────────────────────────────────────────

public sealed class UserPasswordChangedEventHandler
    : INotificationHandler<DomainEventNotification<UserPasswordChangedEvent>>
{
    private readonly ILogger<UserPasswordChangedEventHandler> _logger;
    public UserPasswordChangedEventHandler(ILogger<UserPasswordChangedEventHandler> logger)
        => _logger = logger;

    public Task Handle(DomainEventNotification<UserPasswordChangedEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] Mật khẩu đã thay đổi: UserId={UserId}", n.DomainEvent.UserId);

        // TODO: gửi email cảnh báo bảo mật
        return Task.CompletedTask;
    }
}
