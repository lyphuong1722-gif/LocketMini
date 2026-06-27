namespace LocketMini.Application.Common.Interfaces;

/// <summary>
/// Lấy thông tin người dùng đang đăng nhập từ HTTP context.
/// Implementation nằm ở Infrastructure/Presentation.
/// </summary>
public interface ICurrentUser
{
    int UserId { get; }
    string Username { get; }
    bool IsAuthenticated { get; }
}
