using LocketMini.Application.Features.Auth.Commands;
using LocketMini.Web.Controllers;
using LocketMini.Web.Models;
using LocketSystem.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LocketSystem.Web.Controllers;

public class AuthController : BaseController
{
    // ── GET /Auth/Login ───────────────────────────────────────────────────
    public IActionResult Login(string? returnUrl = null)
    {
        if (IsLoggedIn) return RedirectToAction("Index", "Posts");
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // ── POST /Auth/Login ──────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await Mediator.Send(new LoginCommand(model.Username, model.Password));

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        // Tạo cookie authentication
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.Value.UserId.ToString()),
            new(ClaimTypes.Name,           result.Value.Username),
            new("AccessToken",             result.Value.AccessToken),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = model.RememberMe });

        SetSuccess($"Chào mừng trở lại, {result.Value.Username}!");

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Posts");
    }

    // ── GET /Auth/Register ────────────────────────────────────────────────
    public IActionResult Register()
    {
        if (IsLoggedIn) return RedirectToAction("Index", "Posts");
        return View();
    }

    // ── POST /Auth/Register ───────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await Mediator.Send(
            new RegisterCommand(model.Username, model.Password, model.FullName, model.Bio));

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        // Tự động đăng nhập sau khi đăng ký
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.Value.UserId.ToString()),
            new(ClaimTypes.Name,           result.Value.Username),
            new("AccessToken",             result.Value.AccessToken),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        SetSuccess("Đăng ký thành công! Chào mừng bạn đến với LocketMini 🎉");
        return RedirectToAction("Index", "Posts");
    }

    // ── GET /Auth/ChangePassword ──────────────────────────────────────────
    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult ChangePassword() => View();

    // ── POST /Auth/ChangePassword ─────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await Mediator.Send(
            new ChangePasswordCommand(CurrentUserId, model.CurrentPassword, model.NewPassword));

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        SetSuccess("Đổi mật khẩu thành công!");
        return RedirectToAction("Index", "Posts");
    }

    // ── POST /Auth/Logout ─────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}