using LocketMini.Domain.Exceptions;

namespace LocketMini.Domain.ValueObjects;

public sealed class Username : IEquatable<Username>
{
    public const int MaxLength = 50;
    public const int MinLength = 3;

    public string Value { get; }

    private Username(string value) => Value = value;

    public static Username Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Tên đăng nhập không được trống.");

        value = value.Trim().ToLowerInvariant();

        if (value.Length < MinLength)
            throw new DomainException($"Tên đăng nhập phải có ít nhất {MinLength} ký tự.");

        if (value.Length > MaxLength)
            throw new DomainException($"Tên đăng nhập không được vượt quá {MaxLength} ký tự.");

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-z0-9_]+$"))
            throw new DomainException("Tên đăng nhập chỉ được chứa chữ thường, số và dấu gạch dưới.");

        return new Username(value);
    }

    public bool Equals(Username? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Username u && Equals(u);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static implicit operator string(Username u) => u.Value;
}
