using LocketMini.Domain.Common;
using LocketMini.Domain.Events;
using LocketMini.Domain.Exceptions;
using System.Xml.Linq;

namespace LocketMini.Domain.Entities;

public sealed class Post : BaseEntity
{
    private readonly List<Comment> _comments = new();
    private readonly List<Like> _likes = new();

    public int PostId { get; private set; }
    public int UserId { get; private set; }
    public string? Caption { get; private set; }
    public string? ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public User Owner { get; private set; } = null!;
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();
    public IReadOnlyCollection<Like> Likes => _likes.AsReadOnly();

    private Post() { }

    internal static Post Create(int userId, string? caption, string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(caption) && string.IsNullOrWhiteSpace(imageUrl))
            throw new DomainException("Bài viết phải có caption hoặc ảnh.");

        return new Post
        {
            UserId = userId,
            Caption = caption?.Trim(),
            ImageUrl = imageUrl?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    // ── Behaviours ────────────────────────────────────────────────────────

    public Comment AddComment(int commenterId, string content)
    {
        var comment = Comment.Create(commenterId, PostId, content);
        _comments.Add(comment);
        RaiseDomainEvent(new CommentAddedEvent(PostId, commenterId));
        return comment;
    }

    public Like AddLike(int likerId)
    {
        if (_likes.Any(l => l.UserId == likerId))
            throw new DomainException($"Người dùng {likerId} đã thích bài viết này rồi.");

        var like = Like.Create(likerId, PostId);
        _likes.Add(like);
        RaiseDomainEvent(new PostLikedEvent(PostId, likerId));
        return like;
    }

    public void RemoveLike(int likerId)
    {
        var like = _likes.FirstOrDefault(l => l.UserId == likerId)
            ?? throw new DomainException($"Người dùng {likerId} chưa thích bài viết này.");

        _likes.Remove(like);
        RaiseDomainEvent(new PostUnlikedEvent(PostId, likerId));
    }

    public void UpdateCaption(string? caption)
    {
        Caption = caption?.Trim();
    }

    // ── Computed properties ───────────────────────────────────────────────
    public int LikeCount => _likes.Count;
    public int CommentCount => _comments.Count;
}
