using LocketMini.Application.Common.Interfaces;
using LocketMini.Domain.Interfaces;
using LocketMini.Domain.Interfaces.Repositories;
using LocketMini.Infrastructure.Persistence;
using LocketMini.Infrastructure.Persistence.Repositories;
using LocketMini.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LocketMini.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Đăng ký DB, Repositories, Domain services, Application services, Settings, Seeder.
    /// KHÔNG đăng ký Authentication — để tầng Presentation tự chọn scheme (Cookie / JWT Bearer).
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        // ── Database (SQL Server) ─────────────────────────────────────────
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // ── Unit of Work & Repositories ───────────────────────────────────
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<IFriendRepository, FriendRepository>();

        // ── Domain services ───────────────────────────────────────────────
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        // ── Application services ──────────────────────────────────────────
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<INotificationService, NotificationService>();

        // ── HttpContext & CurrentUser ─────────────────────────────────────
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();

        // ── Settings ──────────────────────────────────────────────────────
        services.Configure<JwtSettings>(config.GetSection(JwtSettings.Section));
        services.Configure<SmtpSettings>(config.GetSection(SmtpSettings.Section));

        // ── Seeder ────────────────────────────────────────────────────────
        services.AddScoped<DbSeeder>();

        return services;
    }

    /// <summary>
    /// Áp dụng migration và seed khi app khởi động.
    /// </summary>
    public static async Task MigrateAndSeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();

        await db.Database.MigrateAsync();
        await seeder.SeedAsync();
    }
}
