namespace LocketMini.Application.Common.Interfaces;

/// <summary>
/// Tạo và xác thực JWT token.
/// Implementation nằm ở Infrastructure.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>Tạo JWT token cho user đã xác thực.</summary>
    string GenerateToken(int userId, string username);

    /// <summary>
    /// Xác thực token. Trả về (UserId, Username) nếu hợp lệ, null nếu không.
    /// </summary>
    (int UserId, string Username)? ValidateToken(string token);
}