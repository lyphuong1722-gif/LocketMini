using LocketMini.Application.Features.Friends.Commands;
using LocketMini.Application.Features.Friends.Queries;
using LocketSystem.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocketSystem.Web.Controllers;

[Authorize]
public class FriendsController : BaseController
{
    // ── GET /Friends ──────────────────────────────────────────────────────
    // Hiển thị đồng thời: danh sách bạn bè, lời mời đã nhận, lời mời đã gửi.
    public async Task<IActionResult> Index()
    {
        var friendsResult = await Mediator.Send(new GetFriendListQuery(CurrentUserId));
        var incomingResult = await Mediator.Send(new GetIncomingFriendRequestsQuery(CurrentUserId));
        var outgoingResult = await Mediator.Send(new GetOutgoingFriendRequestsQuery(CurrentUserId));

        return View(new FriendListViewModel
        {
            Friends = friendsResult.IsSuccess ? friendsResult.Value : [],
            IncomingRequests = incomingResult.IsSuccess ? incomingResult.Value : [],
            OutgoingRequests = outgoingResult.IsSuccess ? outgoingResult.Value : [],
        });
    }

    // ── POST /Friends/Send ────────────────────────────────────────────────
    // Gửi lời mời kết bạn tới targetUserId.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(int targetUserId, string? returnUrl = null)
    {
        var result = await Mediator.Send(new SendFriendRequestCommand(CurrentUserId, targetUserId));

        if (result.IsFailure)
            SetError(result.Error!);
        else
            SetSuccess("Đã gửi lời mời kết bạn!");

        return RedirectBackOrToProfile(targetUserId, returnUrl);
    }

    // ── POST /Friends/Accept ──────────────────────────────────────────────
    // Chấp nhận lời mời kết bạn do requesterId gửi cho mình.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(int requesterId, string? returnUrl = null)
    {
        var result = await Mediator.Send(new AcceptFriendRequestCommand(requesterId, CurrentUserId));

        if (result.IsFailure)
            SetError(result.Error!);
        else
            SetSuccess("Đã chấp nhận lời mời kết bạn!");

        return RedirectBackOrToIndex(returnUrl);
    }

    // ── POST /Friends/Decline ─────────────────────────────────────────────
    // Từ chối lời mời kết bạn do requesterId gửi cho mình.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Decline(int requesterId, string? returnUrl = null)
    {
        var result = await Mediator.Send(new DeclineFriendRequestCommand(requesterId, CurrentUserId));

        if (result.IsFailure)
            SetError(result.Error!);
        else
            SetSuccess("Đã từ chối lời mời kết bạn.");

        return RedirectBackOrToIndex(returnUrl);
    }

    // ── POST /Friends/Cancel ──────────────────────────────────────────────
    // Hủy lời mời kết bạn mà mình đã gửi cho targetUserId (chưa được phản hồi).
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int targetUserId, string? returnUrl = null)
    {
        var result = await Mediator.Send(new CancelFriendRequestCommand(CurrentUserId, targetUserId));

        if (result.IsFailure)
            SetError(result.Error!);
        else
            SetSuccess("Đã hủy lời mời kết bạn.");

        return RedirectBackOrToProfile(targetUserId, returnUrl);
    }

    // ── POST /Friends/Remove ──────────────────────────────────────────────
    // Hủy kết bạn (đã Accepted).
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int targetUserId, string? returnUrl = null)
    {
        var result = await Mediator.Send(new RemoveFriendCommand(CurrentUserId, targetUserId));

        if (result.IsFailure)
            SetError(result.Error!);
        else
            SetSuccess("Đã hủy kết bạn.");

        return RedirectBackOrToProfile(targetUserId, returnUrl);
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

    // ── Helpers ───────────────────────────────────────────────────────────

    private IActionResult RedirectBackOrToProfile(int targetUserId, string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Profile", "Users", new { id = targetUserId });
    }

    private IActionResult RedirectBackOrToIndex(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }
}