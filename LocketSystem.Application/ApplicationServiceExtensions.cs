using System.Reflection;
using FluentValidation;
using LocketMini.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LocketMini.Application;

/// <summary>
/// Đăng ký toàn bộ Application Layer vào DI container.
/// Gọi services.AddApplicationServices() ở Program.cs / Startup.cs.
/// </summary>
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // ── MediatR (handlers, notifications) ────────────────────────────
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        // ── FluentValidation (tự scan tất cả IValidator<T> trong assembly) ─
        services.AddValidatorsFromAssembly(assembly);

        // ── Pipeline behaviors (thứ tự quan trọng) ───────────────────────
        // 1. Logging  → 2. Validation  → handler
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
