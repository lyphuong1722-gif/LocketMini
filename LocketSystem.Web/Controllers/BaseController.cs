using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LocketSystem.Web.Controllers;

public abstract class BaseController : Controller
{
    private ISender? _mediator;
    protected ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected int CurrentUserId =>
        int.TryParse(User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    protected string CurrentUsername =>
        User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName)
        ?? User.FindFirstValue(ClaimTypes.Name)
        ?? string.Empty;

    protected bool IsLoggedIn => User.Identity?.IsAuthenticated ?? false;

    protected void SetError(string message) => TempData["Error"] = message;
    protected void SetSuccess(string message) => TempData["Success"] = message;
}