using LocketMini.Application.Common.Interfaces;
using LocketMini.Domain.Interfaces;
using LocketMini.Domain.Interfaces.Repositories;
using LocketMini.Infrastructure.Persistence;
using LocketMini.Infrastructure.Persistence.Repositories;
using LocketMini.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LocketMini.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
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
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();

        // ── Settings ──────────────────────────────────────────────────────
        services.Configure<JwtSettings>(config.GetSection(JwtSettings.Section));
        services.Configure<SmtpSettings>(config.GetSection(SmtpSettings.Section));

        // ── Seeder ────────────────────────────────────────────────────────
        services.AddScoped<DbSeeder>();

        // ── JWT Authentication ────────────────────────────────────────────
        var jwtSection = config.GetSection(JwtSettings.Section);
        var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey chưa được cấu hình.");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

        return services;
    }

    /// <summary>
    /// Áp dụng migration và seed khi app khởi động.
    /// Gọi trong Program.cs: await app.MigrateAndSeedAsync();
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
