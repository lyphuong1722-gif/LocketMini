using LocketMini.Domain.Common;

namespace LocketMini.Domain.Entities;

public sealed class Like : BaseEntity
{
    public int LikeId { get; private set; }
    public int UserId { get; private set; }
    public int PostId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public User User { get; private set; } = null!;
    public Post Post { get; private set; } = null!;

    private Like() { }

    internal static Like Create(int userId, int postId)
        => new()
        {
            UserId = userId,
            PostId = postId,
            CreatedAt = DateTime.UtcNow
        };
}
