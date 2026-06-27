using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LocketMini.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LocketMini.Infrastructure.Services;

/// <summary>
/// Đọc thông tin user đang đăng nhập từ JWT claim trong HttpContext.
/// </summary>
public sealed class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserService(IHttpContextAccessor http) => _http = http;

    private ClaimsPrincipal? Principal => _http.HttpContext?.User;

    public bool IsAuthenticated
        => Principal?.Identity?.IsAuthenticated ?? false;

    public int UserId
    {
        get
        {
            var value = Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out var id) ? id : 0;
        }
    }

    public string Username
        => Principal?.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
        ?? Principal?.FindFirstValue(ClaimTypes.Name)
        ?? string.Empty;
}
