using System.Globalization;
using System.Text;

namespace LocketMini.Infrastructure.Common;

/// <summary>
/// Chuẩn hóa chuỗi để so sánh tìm kiếm: bỏ dấu tiếng Việt + chuyển về chữ thường.
/// </summary>
public static class TextNormalizer
{
    public static string RemoveDiacritics(string? text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        // "đ"/"Đ" không tự tách dấu qua NFD nên xử lý riêng
        text = text.Replace('đ', 'd').Replace('Đ', 'D');

        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>Chuẩn hóa để so sánh: bỏ dấu + lowercase.</summary>
    public static string NormalizeForSearch(string? text)
        => RemoveDiacritics(text).ToLowerInvariant();
}