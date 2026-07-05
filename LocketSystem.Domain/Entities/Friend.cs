using LocketMini.Domain.Common;

namespace LocketMini.Domain.Entities;

/// <summary>Trạng thái của một dòng quan hệ bạn bè.</summary>
public enum FriendStatus
{
    /// <summary>Lời mời kết bạn đã gửi, chưa được phản hồi.</summary>
    Pending = 0,

    /// <summary>Đã chấp nhận — hai bên là bạn bè.</summary>
    Accepted = 1
}

/// <summary>
/// Đại diện cho một chiều của mối quan hệ bạn bè / lời mời kết bạn.
/// Bảng Friends trong DB là composite PK (user_id, friend_id).
///
/// - Khi A gửi lời mời cho B: tạo 1 dòng (UserId=A, FriendId=B, Status=Pending).
/// - Khi B chấp nhận: dòng trên chuyển Status=Accepted, đồng thời tạo thêm
///   1 dòng (UserId=B, FriendId=A, Status=Accepted) để có thể truy vấn
///   danh sách bạn bè từ cả hai phía một cách đối xứng.
/// - Khi từ chối / hủy lời mời: xóa dòng Pending tương ứng.
/// - Khi hủy kết bạn: xóa CẢ HAI dòng Accepted.
/// </summary>
public sealed class Friend : BaseEntity
{
    public int UserId { get; private set; }
    public int FriendId { get; private set; }
    public FriendStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RespondedAt { get; private set; }

    // Navigation
    public User User { get; private set; } = null!;
    public User FriendUser { get; private set; } = null!;

    private Friend() { }

    /// <summary>Tạo dòng lời mời kết bạn mới (Pending).</summary>
    internal static Friend Create(int userId, int friendId)
        => new()
        {
            UserId = userId,
            FriendId = friendId,
            Status = FriendStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

    /// <summary>Tạo trực tiếp dòng bạn bè đã chấp nhận (dùng cho dòng đối xứng khi Accept, hoặc seed data).</summary>
    internal static Friend CreateAccepted(int userId, int friendId)
        => new()
        {
            UserId = userId,
            FriendId = friendId,
            Status = FriendStatus.Accepted,
            CreatedAt = DateTime.UtcNow,
            RespondedAt = DateTime.UtcNow
        };

    internal void Accept()
    {
        Status = FriendStatus.Accepted;
        RespondedAt = DateTime.UtcNow;
    }
}