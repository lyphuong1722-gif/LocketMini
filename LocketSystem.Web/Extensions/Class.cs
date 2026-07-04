namespace LocketSystem.Web.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Chuyển đường dẫn lưu trong DB (vd: "wwwroot/images/abc.jpg")
    /// thành đường dẫn web hợp lệ (vd: "/images/abc.jpg").
    /// </summary>
    public static string ToWebPath(this string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        var normalized = path.Replace('\\', '/');

        if (normalized.StartsWith("wwwroot/", StringComparison.OrdinalIgnoreCase))
            normalized = normalized["wwwroot".Length..]; // bỏ "wwwroot", giữ lại "/images/abc.jpg"

        return normalized.StartsWith("/") ? normalized : "/" + normalized;
    }
}