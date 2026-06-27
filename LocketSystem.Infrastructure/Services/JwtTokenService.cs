using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LocketMini.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LocketMini.Infrastructure.Services;

// ── Settings ──────────────────────────────────────────────────────────────────

public sealed class JwtSettings
{
    public const string Section = "JwtSettings";

    public string SecretKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpiryDays { get; init; } = 7;
}

// ── Service ───────────────────────────────────────────────────────────────────

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> options)
        => _settings = options.Value;

    public string GenerateToken(int userId, string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,            userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName,     username),
            new Claim(JwtRegisteredClaimNames.Jti,            Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                      DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                      ClaimValueTypes.Integer64),
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(_settings.ExpiryDays),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (int UserId, string Username)? ValidateToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.SecretKey);

            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            }, out _);

            var userIdStr = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var username = principal.FindFirstValue(JwtRegisteredClaimNames.UniqueName);

            if (userIdStr is null || username is null || !int.TryParse(userIdStr, out var userId))
                return null;

            return (userId, username);
        }
        catch
        {
            return null;
        }
    }
}
