using System.ComponentModel.DataAnnotations;
using LocketMini.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace LocketSystem.Web.Models;

// ── Auth ViewModels ───────────────────────────────────────────────────────────

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ghi nhớ đăng nhập")]
    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3–50 ký tự.")]
    [RegularExpression(@"^[a-z0-9_]+$", ErrorMessage = "Chỉ được dùng chữ thường, số và dấu gạch dưới.")]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [DataType(DataType.Password)]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [StringLength(100)]
    [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Họ và tên chỉ được chứa chữ cái.")]
    [Display(Name = "Họ và tên")]
    public string? FullName { get; set; }

    [StringLength(500)]
    [Display(Name = "Giới thiệu bản thân")]
    public string? Bio { get; set; }
}

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu hiện tại")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [DataType(DataType.Password)]
    [Display(Name = "Xác nhận mật khẩu mới")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

// ── Post ViewModels ───────────────────────────────────────────────────────────

public class FeedViewModel
{
    public IReadOnlyList<PostDto> Posts { get; set; } = [];
    public int Page { get; set; } = 1;
    public bool HasNext { get; set; }
    public bool HasPrev { get; set; }
}

public class PostDetailViewModel
{
    public PostDto Post { get; set; } = null!;
    public IReadOnlyList<CommentDto> Comments { get; set; } = [];
}

public class CreatePostViewModel
{
    [StringLength(2000)]
    [Display(Name = "Caption")]
    public string? Caption { get; set; }

    [Display(Name = "Chọn ảnh từ máy")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "URL ảnh")]
    [Url(ErrorMessage = "URL ảnh không hợp lệ.")]
    public string? ImageUrl { get; set; }
}

// ── User ViewModels ───────────────────────────────────────────────────────────

public class UserProfileViewModel
{
    public UserDto User { get; set; } = null!;
    public IReadOnlyList<PostDto> Posts { get; set; } = [];

    /// <summary>Trạng thái quan hệ giữa người đang đăng nhập và chủ trang cá nhân này.</summary>
    public FriendshipRelation Relation { get; set; } = FriendshipRelation.None;

    public bool IsMyself { get; set; }
    public int FriendCount { get; set; }
}

public class SearchUsersViewModel
{
    public string Keyword { get; set; } = string.Empty;
    public IReadOnlyList<UserSummaryDto> Users { get; set; } = [];
    public int Page { get; set; } = 1;
    public bool HasNext { get; set; }
    public bool HasPrev { get; set; }
}

// ── Friend ViewModels ─────────────────────────────────────────────────────────

public class FriendListViewModel
{
    /// <summary>Danh sách bạn bè đã kết bạn (Accepted).</summary>
    public IReadOnlyList<FriendDto> Friends { get; set; } = [];

    /// <summary>Lời mời kết bạn tôi ĐÃ NHẬN, đang chờ tôi phản hồi.</summary>
    public IReadOnlyList<FriendRequestDto> IncomingRequests { get; set; } = [];

    /// <summary>Lời mời kết bạn tôi ĐÃ GỬI, đang chờ đối phương phản hồi.</summary>
    public IReadOnlyList<FriendRequestDto> OutgoingRequests { get; set; } = [];
}

public class MutualFriendsViewModel
{
    public int OtherUserId { get; set; }
    public IReadOnlyList<FriendDto> Friends { get; set; } = [];
}