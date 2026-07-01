using LocketMini.Application.Common.Behaviors;
using LocketMini.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// ── MVC (Razor Views) ─────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── MediatR + Pipeline Behaviors ─────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<
        LocketMini.Application.Features.Auth.Commands.LoginCommand>();

    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// ── FluentValidation ──────────────────────────────────────────────────────────
builder.Services.AddValidatorsFromAssemblyContaining<
    LocketMini.Application.Features.Auth.Commands.LoginCommandValidator>();

// ── Infrastructure: DB, Repos, Services, Settings ────────────────────────────
// Gọi extension method — KHÔNG đăng ký JWT Bearer Authentication bên trong
builder.Services.AddInfrastructureServices(builder.Configuration);

// ── Cookie Authentication (override scheme mặc định của Infrastructure) ───────
// Infrastructure đăng ký JWT Bearer, ta override lại DefaultScheme thành Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "LocketMini.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy =
            Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Error handling ────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}");

// ── Migrate DB & Seed ─────────────────────────────────────────────────────────
await app.Services.MigrateAndSeedAsync();

app.Run();