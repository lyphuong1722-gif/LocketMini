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

// ── FriendRequestSent ─────────────────────────────────────────────────────────

public sealed class FriendRequestSentEventHandler
    : INotificationHandler<DomainEventNotification<FriendRequestSentEvent>>
{
    private readonly ILogger<FriendRequestSentEventHandler> _logger;
    public FriendRequestSentEventHandler(ILogger<FriendRequestSentEventHandler> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<FriendRequestSentEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] UserId={RequesterId} gửi lời mời kết bạn tới UserId={TargetId}",
            n.DomainEvent.RequesterId, n.DomainEvent.TargetId);

        // TODO: notify người nhận lời mời
        return Task.CompletedTask;
    }
}

// ── FriendRequestAccepted ─────────────────────────────────────────────────────

public sealed class FriendRequestAcceptedEventHandler
    : INotificationHandler<DomainEventNotification<FriendRequestAcceptedEvent>>
{
    private readonly ILogger<FriendRequestAcceptedEventHandler> _logger;
    public FriendRequestAcceptedEventHandler(ILogger<FriendRequestAcceptedEventHandler> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<FriendRequestAcceptedEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] UserId={AccepterId} đã chấp nhận lời mời kết bạn từ UserId={RequesterId}",
            n.DomainEvent.AccepterId, n.DomainEvent.RequesterId);

        // TODO: notify người gửi lời mời ban đầu
        return Task.CompletedTask;
    }
}

// ── FriendRequestDeclined ─────────────────────────────────────────────────────

public sealed class FriendRequestDeclinedEventHandler
    : INotificationHandler<DomainEventNotification<FriendRequestDeclinedEvent>>
{
    private readonly ILogger<FriendRequestDeclinedEventHandler> _logger;
    public FriendRequestDeclinedEventHandler(ILogger<FriendRequestDeclinedEventHandler> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<FriendRequestDeclinedEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] Lời mời kết bạn từ UserId={RequesterId} tới UserId={TargetId} đã bị từ chối",
            n.DomainEvent.RequesterId, n.DomainEvent.TargetId);
        return Task.CompletedTask;
    }
}

// ── FriendRequestCancelled ────────────────────────────────────────────────────

public sealed class FriendRequestCancelledEventHandler
    : INotificationHandler<DomainEventNotification<FriendRequestCancelledEvent>>
{
    private readonly ILogger<FriendRequestCancelledEventHandler> _logger;
    public FriendRequestCancelledEventHandler(ILogger<FriendRequestCancelledEventHandler> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<FriendRequestCancelledEvent> n, CancellationToken ct)
    {
        _logger.LogInformation(
            "[Event] UserId={RequesterId} đã hủy lời mời kết bạn gửi tới UserId={TargetId}",
            n.DomainEvent.RequesterId, n.DomainEvent.TargetId);
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