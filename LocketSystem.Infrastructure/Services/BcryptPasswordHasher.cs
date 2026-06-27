using LocketMini.Domain.Interfaces;

namespace LocketMini.Infrastructure.Services;

/// <summary>
/// BCrypt password hasher.
/// NuGet: BCrypt.Net-Next
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Mật khẩu không được trống.", nameof(plainPassword));

        return BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);
    }

    public bool Verify(string plainPassword, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
    }
}
