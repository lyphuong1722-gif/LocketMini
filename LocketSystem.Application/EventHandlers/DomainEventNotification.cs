using LocketMini.Domain.Common;
using MediatR;

namespace LocketMini.Application.EventHandlers;

/// <summary>
/// Adapter bọc IDomainEvent thành INotification của MediatR.
/// Infrastructure sẽ publish các domain event qua class này sau SaveChanges.
/// </summary>
public sealed class DomainEventNotification<TDomainEvent> : INotification
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; }

    public DomainEventNotification(TDomainEvent domainEvent)
        => DomainEvent = domainEvent;
}
