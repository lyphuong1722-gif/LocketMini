using LocketMini.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace LocketMini.Infrastructure.Services;

/// <summary>
/// Stub implementation của INotificationService.
/// Thay bằng Firebase Cloud Messaging hoặc SignalR khi cần.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger) => _logger = logger;

    public Task NotifyNewPostAsync(
        int postOwnerId, IEnumerable<int> friendIds, int postId, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[Notify] Bài viết mới PostId={PostId} của UserId={OwnerId} → gửi tới {Count} bạn bè",
            postId, postOwnerId, friendIds.Count());

        // TODO: gọi Firebase Admin SDK hoặc push qua SignalR hub
        return Task.CompletedTask;
    }

    public Task NotifyPostLikedAsync(
        int postOwnerId, int likerId, int postId, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[Notify] PostId={PostId} được thích bởi UserId={LikerId} → notify OwnerId={OwnerId}",
            postId, likerId, postOwnerId);
        return Task.CompletedTask;
    }

    public Task NotifyCommentAddedAsync(
        int postOwnerId, int commenterId, int postId, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[Notify] Bình luận mới trên PostId={PostId} bởi UserId={CommenterId} → notify OwnerId={OwnerId}",
            postId, commenterId, postOwnerId);
        return Task.CompletedTask;
    }

    public Task NotifyFriendRequestAsync(
        int targetUserId, int requesterId, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[Notify] UserId={RequesterId} muốn kết bạn với UserId={TargetId}",
            requesterId, targetUserId);
        return Task.CompletedTask;
    }
}
