namespace LocketMini.Application.Common.Interfaces;

/// <summary>
/// Push notification tới người dùng (Firebase, SignalR, …).
/// Implementation nằm ở Infrastructure.
/// </summary>
public interface INotificationService
{
    Task NotifyNewPostAsync(
        int postOwnerId,
        IEnumerable<int> friendIds,
        int postId,
        CancellationToken ct = default);

    Task NotifyPostLikedAsync(
        int postOwnerId,
        int likerId,
        int postId,
        CancellationToken ct = default);

    Task NotifyCommentAddedAsync(
        int postOwnerId,
        int commenterId,
        int postId,
        CancellationToken ct = default);

    Task NotifyFriendRequestAsync(
        int targetUserId,
        int requesterId,
        CancellationToken ct = default);
}