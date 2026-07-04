using LocketMini.Application.Features.Posts.Commands;
using LocketMini.Application.Features.Posts.Queries;
using LocketMini.Application.Features.Comments.Queries;
using LocketSystem.Application.Features.Comments.Commands;
using LocketSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace LocketSystem.Web.Controllers;

[Authorize]
public class PostsController : BaseController
{
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedImageExtensions =
        { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private const long MaxImageSizeBytes = 10 * 1024 * 1024; // 10 MB

    public PostsController(IWebHostEnvironment env)
    {
        _env = env;
    }

    // ── GET /Posts  (Feed) ────────────────────────────────────────────────
    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await Mediator.Send(new GetFeedQuery(CurrentUserId, page, PageSize: 10));
        if (result.IsFailure)
        {
            SetError(result.Error!);
            return View(new FeedViewModel());
        }

        return View(new FeedViewModel
        {
            Posts = result.Value.Items,
            Page = result.Value.Page,
            HasNext = result.Value.HasNextPage,
            HasPrev = result.Value.HasPrevPage,
        });
    }

    // ── GET /Posts/Detail/5 ───────────────────────────────────────────────
    public async Task<IActionResult> Detail(int id)
    {
        var postResult = await Mediator.Send(new GetPostDetailQuery(CurrentUserId, id));
        if (postResult.IsFailure) return NotFound();

        var commentsResult = await Mediator.Send(new GetCommentsByPostQuery(id));

        return View(new PostDetailViewModel
        {
            Post = postResult.Value,
            Comments = commentsResult.IsSuccess ? commentsResult.Value : [],
        });
    }

    // ── GET /Posts/Create ─────────────────────────────────────────────────
    public IActionResult Create() => View();

    // ── POST /Posts/Create ────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePostViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var imageUrl = model.ImageUrl;

        // Nếu người dùng chọn file ảnh từ máy, ưu tiên lưu file này
        // và dùng đường dẫn web tương ứng làm ImageUrl.
        if (model.ImageFile is { Length: > 0 })
        {
            var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();

            if (!AllowedImageExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(model.ImageFile),
                    "Định dạng ảnh không được hỗ trợ (chỉ chấp nhận JPG, PNG, GIF, WEBP).");
                return View(model);
            }

            if (model.ImageFile.Length > MaxImageSizeBytes)
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Ảnh không được vượt quá 10MB.");
                return View(model);
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            imageUrl = $"/uploads/posts/{fileName}";
        }

        var result = await Mediator.Send(
            new CreatePostCommand(CurrentUserId, model.Caption, imageUrl));

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        SetSuccess("Đăng bài thành công!");
        return RedirectToAction(nameof(Detail), new { id = result.Value });
    }

    // ── POST /Posts/Delete ────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await Mediator.Send(new DeletePostCommand(CurrentUserId, id));
        if (result.IsFailure) SetError(result.Error!);
        else SetSuccess("Đã xóa bài viết.");

        return RedirectToAction(nameof(Index));
    }

    // ── POST /Posts/Like ──────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Like(int id, string? ret = "Index")
    {
        var result = await Mediator.Send(new LikePostCommand(CurrentUserId, id));
        if (result.IsFailure) SetError(result.Error!);
        return ret == "Detail"
            ? RedirectToAction(nameof(Detail), new { id })
            : RedirectToAction(nameof(Index));
    }

    // ── POST /Posts/Unlike ────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlike(int id, string? ret = "Index")
    {
        var result = await Mediator.Send(new UnlikePostCommand(CurrentUserId, id));
        if (result.IsFailure) SetError(result.Error!);
        return ret == "Detail"
            ? RedirectToAction(nameof(Detail), new { id })
            : RedirectToAction(nameof(Index));
    }

    // ── POST /Posts/AddComment ────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int postId, string content)
    {
        if (!string.IsNullOrWhiteSpace(content))
        {
            var result = await Mediator.Send(new AddCommentCommand(CurrentUserId, postId, content));
            if (result.IsFailure) SetError(result.Error!);
        }
        return RedirectToAction(nameof(Detail), new { id = postId });
    }

    // ── POST /Posts/DeleteComment ─────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(int commentId, int postId)
    {
        var result = await Mediator.Send(new DeleteCommentCommand(CurrentUserId, commentId));
        if (result.IsFailure) SetError(result.Error!);
        return RedirectToAction(nameof(Detail), new { id = postId });
    }

    // ── POST /Posts/EditComment ───────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditComment(int commentId, int postId, string newContent)
    {
        var result = await Mediator.Send(new EditCommentCommand(CurrentUserId, commentId, newContent));
        if (result.IsFailure) SetError(result.Error!);
        return RedirectToAction(nameof(Detail), new { id = postId });
    }

    // ── GET /Posts/Stats/5 ────────────────────────────────────────────────
    public async Task<IActionResult> Stats(int id)
    {
        var result = await Mediator.Send(new GetPostStatsQuery(id));
        if (result.IsFailure) return NotFound();
        return View(result.Value);
    }
}