namespace LocketMini.Application.DTOs;

// ── Auth ──────────────────────────────────────────────────────────────────────

public sealed record AuthTokenDto(
    int UserId,
    string Username,
    string AccessToken);

// ── User / Profile ────────────────────────────────────────────────────────────

public sealed record UserDto(
    int UserId,
    string Username,
    string? FullName,
    string? Bio);

public sealed record UserSummaryDto(
    int UserId,
    string Username,
    string? FullName);

// ── Post ──────────────────────────────────────────────────────────────────────

public sealed record PostDto(
    int PostId,
    int UserId,
    string Username,
    string? Caption,
    string? ImageUrl,
    DateTime CreatedAt,
    int LikeCount,
    int CommentCount,
    bool LikedByMe);

public sealed record PostStatsDto(
    int PostId,
    string Username,
    string? Caption,
    string? ImageUrl,
    int LikeCount,
    int CommentCount);

// ── Comment ───────────────────────────────────────────────────────────────────

public sealed record CommentDto(
    int CommentId,
    int UserId,
    string Username,
    string Content,
    DateTime CreatedAt);

// ── Like ──────────────────────────────────────────────────────────────────────

public sealed record LikeDto(
    int LikeId,
    int UserId,
    string Username,
    DateTime CreatedAt);

// ── Friend ────────────────────────────────────────────────────────────────────

/// <summary>Đại diện cho một người bạn đã kết bạn (Accepted).</summary>
public sealed record FriendDto(
    int UserId,
    string Username,
    string? FullName);

/// <summary>
/// Đại diện cho MỘT PHÍA của lời mời kết bạn đang chờ (Pending):
/// - Với lời mời ĐẾN: đây là thông tin của người GỬI.
/// - Với lời mời ĐI: đây là thông tin của người NHẬN.
/// </summary>
public sealed record FriendRequestDto(
    int UserId,
    string Username,
    string? FullName,
    DateTime SentAt);

/// <summary>Trạng thái quan hệ giữa "tôi" và một user khác.</summary>
public enum FriendshipRelation
{
    /// <summary>Chưa có quan hệ gì.</summary>
    None,

    /// <summary>Tôi đã gửi lời mời kết bạn, đang chờ họ phản hồi.</summary>
    PendingSentByMe,

    /// <summary>Họ đã gửi lời mời kết bạn cho tôi, đang chờ tôi phản hồi.</summary>
    PendingReceivedByMe,

    /// <summary>Đã là bạn bè.</summary>
    Friends
}

// ── Pagination ────────────────────────────────────────────────────────────────

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPrevPage => Page > 1;
}