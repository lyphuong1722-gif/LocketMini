using LocketMini.Application.Features.Friends.Commands;
using LocketMini.Application.Features.Friends.Queries;
using LocketMini.Web.Controllers;
using LocketMini.Web.Models;
using LocketSystem.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocketSystem.Web.Controllers;

[Authorize]
public class FriendsController : BaseController
{
    // ── GET /Friends ──────────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        var result = await Mediator.Send(new GetFriendListQuery(CurrentUserId));

        return View(new FriendListViewModel
        {
            Friends = result.IsSuccess ? result.Value : [],
        });
    }

    // ── POST /Friends/Add ─────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int targetUserId, string? returnUrl = null)
    {
        var result = await Mediator.Send(new AddFriendCommand(CurrentUserId, targetUserId));

        if (result.IsFailure)
            SetError(result.Error!);
        else
            SetSuccess("Đã thêm bạn bè thành công!");

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Profile", "Users", new { id = targetUserId });
    }

    // ── POST /Friends/Remove ──────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int targetUserId, string? returnUrl = null)
    {
        var result = await Mediator.Send(new RemoveFriendCommand(CurrentUserId, targetUserId));

        if (result.IsFailure)
            SetError(result.Error!);
        else
            SetSuccess("Đã hủy kết bạn.");

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    // ── GET /Friends/Mutual/5 ─────────────────────────────────────────────
    public async Task<IActionResult> Mutual(int userId)
    {
        var result = await Mediator.Send(new GetMutualFriendsQuery(CurrentUserId, userId));

        return View(new MutualFriendsViewModel
        {
            OtherUserId = userId,
            Friends = result.IsSuccess ? result.Value : [],
        });
    }
}