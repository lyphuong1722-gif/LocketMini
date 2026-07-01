using MediatR;

namespace LocketMini.Application.Common.Interfaces;

// ── Commands ──────────────────────────────────────────────────────────────────

/// <summary>Command không trả dữ liệu (chỉ trả Result).</summary>
public interface ICommand : IRequest<Result> { }

/// <summary>Command trả dữ liệu.</summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }

// ── Queries ───────────────────────────────────────────────────────────────────

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }

// ── Handlers ─────────────────────────────────────────────────────────────────

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{ }

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{ }

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{ }

// ── MediatR re-exports (để không cần using MediatR khắp nơi) ─────────────────
// Các interface trên kế thừa IRequest / IRequestHandler từ MediatR.
// Project cần cài: MediatR, FluentValidation, FluentValidation.DependencyInjectionExtensions
