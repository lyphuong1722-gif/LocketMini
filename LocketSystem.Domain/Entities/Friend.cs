using LocketMini.Domain.Common;

namespace LocketMini.Domain.Entities;

/// <summary>
/// Đại diện cho một chiều của mối quan hệ bạn bè.
/// Bảng Friends trong DB là composite PK (user_id, friend_id).
/// </summary>
public sealed class Friend : BaseEntity
{
    public int UserId { get; private set; }
    public int FriendId { get; private set; }

    // Navigation
    public User User { get; private set; } = null!;
    public User FriendUser { get; private set; } = null!;

    private Friend() { }

    internal static Friend Create(int userId, int friendId)
        => new() { UserId = userId, FriendId = friendId };
}
