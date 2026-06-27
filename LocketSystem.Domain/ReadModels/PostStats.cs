namespace LocketMini.Domain.ReadModels;

/// <summary>
/// Read model tương ứng với truy vấn thống kê bài viết trong SQL.
/// Dùng ở tầng Application (query side), không phải Entity.
/// </summary>
public sealed record PostStats(
    int PostId,
    string Username,
    string? Caption,
    string? ImageUrl,
    int LikeCount,
    int CommentCount
);
