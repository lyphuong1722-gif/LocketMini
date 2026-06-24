using LocketMini.Domain.Common;

namespace LocketMini.Domain.Entities;

public sealed class Profile : BaseEntity
{
    public int ProfileId { get; private set; }
    public int UserId { get; private set; }
    public string? FullName { get; private set; }
    public string? Bio { get; private set; }

    // Navigation
    public User User { get; private set; } = null!;

    private Profile() { }

    internal static Profile Create(int userId, string? fullName, string? bio)
        => new()
        {
            UserId = userId,
            FullName = fullName?.Trim(),
            Bio = bio?.Trim()
        };

    internal void Update(string? fullName, string? bio)
    {
        FullName = fullName?.Trim();
        Bio = bio?.Trim();
    }
}
