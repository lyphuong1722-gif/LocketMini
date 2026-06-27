namespace LocketMini.Domain.Interfaces;

/// <summary>
/// Domain service: hashing mật khẩu.
/// Implementation nằm ở Infrastructure để giữ Domain không phụ thuộc thư viện ngoài.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string plainPassword);
    bool Verify(string plainPassword, string hashedPassword);
}
