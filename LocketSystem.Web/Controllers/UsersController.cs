using LocketMini.Application.Features.Friends.Commands;
using LocketMini.Application.Features.Friends.Queries;
using LocketMini.Application.Features.Posts.Queries;
using LocketMini.Application.Features.Users.Queries;
using LocketSystem.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocketSystem.Web.Controllers;

[Authorize]
public class UsersController : BaseController
{
    // ── GET /Users/Profile/5 ──────────────────────────────────────────────
    public async Task<IActionResult> Profile(int id)
    {
        var userResult = await Mediator.Send(new GetUserProfileQuery(id));
        if (userResult.IsFailure) return NotFound();

        var postsResult = await Mediator.Send(new GetUserPostsQuery(CurrentUserId, id));
        // CheckFriendshipCommand trả Result<bool>
        var friendResult = await Mediator.Send(new CheckFriendshipCommand(CurrentUserId, id));
        // Danh sách bạn bè của user đang xem để đếm số lượng
        var friendListResult = await Mediator.Send(new GetFriendListQuery(id));

        return View(new UserProfileViewModel
        {
            User = userResult.Value,
            Posts = postsResult.IsSuccess ? postsResult.Value : [],
            IsFriend = friendResult.IsSuccess && friendResult.Value,
            IsMyself = id == CurrentUserId,
            FriendCount = friendListResult.IsSuccess ? friendListResult.Value.Count : 0,
        });
    }

    // ── GET /Users/Me ─────────────────────────────────────────────────────
    public IActionResult Me() => RedirectToAction(nameof(Profile), new { id = CurrentUserId });

    // ── GET /Users/Search?keyword=... ─────────────────────────────────────
    public async Task<IActionResult> Search(string? keyword, int page = 1)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return View(new SearchUsersViewModel { Keyword = string.Empty });

        var result = await Mediator.Send(new SearchUsersQuery(keyword, page, 20));

        return View(new SearchUsersViewModel
        {
            Keyword = keyword,
            Users = result.IsSuccess ? result.Value.Items : [],
            Page = result.IsSuccess ? result.Value.Page : 1,
            HasNext = result.IsSuccess && result.Value.HasNextPage,
            HasPrev = result.IsSuccess && result.Value.HasPrevPage,
        });
    }
}
