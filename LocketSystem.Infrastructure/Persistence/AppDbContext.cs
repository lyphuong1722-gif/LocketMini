using LocketMini.Application.EventHandlers;
using LocketMini.Domain.Common;
using LocketMini.Domain.Entities;
using LocketMini.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LocketMini.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    private readonly IMediator _mediator;

    public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    // ── DbSets ────────────────────────────────────────────────────────────
    public DbSet<User> Users => Set<User>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Like> Likes => Set<Like>();
    public DbSet<Friend> Friends => Set<Friend>();

    // ── Model configuration ───────────────────────────────────────────────
    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(mb);
    }

    // ── SaveChanges: dispatch domain events sau khi lưu thành công ────────
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // Thu thập tất cả domain events trước khi lưu
        var entitiesWithEvents = ChangeTracker
            .Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Xóa events trước SaveChanges để tránh dispatch lại nếu retry
        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        var result = await base.SaveChangesAsync(ct);

        // Dispatch sau khi lưu thành công
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = Activator.CreateInstance(notificationType, domainEvent)!;
            await _mediator.Publish(notification, ct);
        }

        return result;
    }
}
