namespace LocketSystem.Web.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Chuyển đường dẫn lưu trong DB thành đường dẫn web hợp lệ.
    /// - Ảnh upload từ máy: lưu dạng "wwwroot/uploads/..." hoặc "/uploads/..." 
    ///   -> chuẩn hóa thành đường dẫn tương đối bắt đầu bằng "/".
    /// - Ảnh dán URL từ mạng: đã là URL tuyệt đối (http/https)
    ///   -> giữ nguyên, KHÔNG được nối thêm "/" ở đầu, nếu không sẽ tạo
    ///      thành đường dẫn sai (vd "/https://example.com/anh.jpg") khiến
    ///      trình duyệt không tải được ảnh.
    /// </summary>
    public static string ToWebPath(this string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        // URL tuyệt đối (http/https) -> giữ nguyên, không xử lý gì thêm
        if (Uri.TryCreate(path, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return path;
        }

        var normalized = path.Replace('\\', '/');

        if (normalized.StartsWith("wwwroot/", StringComparison.OrdinalIgnoreCase))
            normalized = normalized["wwwroot".Length..]; // bỏ "wwwroot", giữ lại "/images/abc.jpg"

        return normalized.StartsWith("/") ? normalized : "/" + normalized;
    }
}