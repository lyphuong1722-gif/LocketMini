using LocketMini.Domain.Common;
using LocketMini.Domain.Exceptions;

namespace LocketMini.Domain.Entities;

public sealed class Comment : BaseEntity
{
    public int CommentId { get; private set; }
    public int UserId { get; private set; }
    public int PostId { get; private set; }
    public string Content { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public User User { get; private set; } = null!;
    public Post Post { get; private set; } = null!;

    private Comment() { }

    internal static Comment Create(int userId, int postId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Nội dung bình luận không được trống.");

        if (content.Length > 1000)
            throw new DomainException("Bình luận không được vượt quá 1000 ký tự.");

        return new Comment
        {
            UserId = userId,
            PostId = postId,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void EditContent(int requesterId, string newContent)
    {
        if (requesterId != UserId)
            throw new DomainException("Chỉ tác giả mới có thể chỉnh sửa bình luận này.");

        if (string.IsNullOrWhiteSpace(newContent))
            throw new DomainException("Nội dung bình luận không được trống.");

        Content = newContent.Trim();
    }
}
