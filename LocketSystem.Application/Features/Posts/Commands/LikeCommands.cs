using LocketMini.Application.Common;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketMini.Application.Features.Posts.Commands;

// ── Like ──────────────────────────────────────────────────────────────────────

public sealed record LikePostCommand(int RequesterId, int PostId) : IRequest<Result>;

public sealed class LikePostHandler : IRequestHandler<LikePostCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public LikePostHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(LikePostCommand request, CancellationToken ct)
    {
        var post = await _uow.Posts.GetWithDetailsAsync(request.PostId, ct)
            ?? throw new NotFoundException("Post", request.PostId);

        try
        {
            post.AddLike(request.RequesterId);
            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}

// ── Unlike ────────────────────────────────────────────────────────────────────

public sealed record UnlikePostCommand(int RequesterId, int PostId) : IRequest<Result>;

public sealed class UnlikePostHandler : IRequestHandler<UnlikePostCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public UnlikePostHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(UnlikePostCommand request, CancellationToken ct)
    {
        var post = await _uow.Posts.GetWithDetailsAsync(request.PostId, ct)
            ?? throw new NotFoundException("Post", request.PostId);

        try
        {
            post.RemoveLike(request.RequesterId);
            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
