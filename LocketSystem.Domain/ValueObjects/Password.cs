using LocketMini.Domain.Exceptions;

namespace LocketMini.Domain.ValueObjects;

/// <summary>
/// Lưu trữ mật khẩu đã được hash.
/// Việc hash thực sự được thực hiện ở tầng Application/Infrastructure.
/// </summary>
public sealed class Password : IEquatable<Password>
{
    public string HashedValue { get; }

    private Password(string hashedValue) => HashedValue = hashedValue;

    /// <summary>Tạo Password từ chuỗi hash đã có.</summary>
    public static Password Create(string hashedValue)
    {
        if (string.IsNullOrWhiteSpace(hashedValue))
            throw new DomainException("Mật khẩu không hợp lệ.");

        return new Password(hashedValue);
    }

    public bool Equals(Password? other) => other is not null && HashedValue == other.HashedValue;
    public override bool Equals(object? obj) => obj is Password p && Equals(p);
    public override int GetHashCode() => HashedValue.GetHashCode();

    // Ẩn giá trị hash khi toString để tránh lộ trong log
    public override string ToString() => "***";
}
