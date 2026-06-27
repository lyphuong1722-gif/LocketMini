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

public sealed record FriendDto(
    int UserId,
    string Username,
    string? FullName);

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
