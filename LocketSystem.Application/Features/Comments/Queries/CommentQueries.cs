using LocketMini.Application.Common;
using LocketMini.Application.DTOs;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketMini.Application.Features.Comments.Queries;

// ── GetCommentsByPost ─────────────────────────────────────────────────────────

public sealed record GetCommentsByPostQuery(int PostId) : IRequest<Result<IReadOnlyList<CommentDto>>>;

public sealed class GetCommentsByPostHandler
    : IRequestHandler<GetCommentsByPostQuery, Result<IReadOnlyList<CommentDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetCommentsByPostHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IReadOnlyList<CommentDto>>> Handle(
        GetCommentsByPostQuery request, CancellationToken ct)
    {
        // Đảm bảo post tồn tại
        var post = await _uow.Posts.GetByIdAsync(request.PostId, ct)
            ?? throw new NotFoundException("Post", request.PostId);

        var comments = await _uow.Comments.GetByPostAsync(request.PostId, ct);

        var dtos = comments
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto(
                c.CommentId,
                c.UserId,
                c.User?.Username.Value ?? string.Empty,
                c.Content,
                c.CreatedAt))
            .ToList();

        return Result.Success<IReadOnlyList<CommentDto>>(dtos);
    }
}

// ── GetLikersByPost ───────────────────────────────────────────────────────────

public sealed record GetLikersByPostQuery(int PostId) : IRequest<Result<IReadOnlyList<LikeDto>>>;

public sealed class GetLikersByPostHandler
    : IRequestHandler<GetLikersByPostQuery, Result<IReadOnlyList<LikeDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetLikersByPostHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IReadOnlyList<LikeDto>>> Handle(
        GetLikersByPostQuery request, CancellationToken ct)
    {
        var post = await _uow.Posts.GetWithDetailsAsync(request.PostId, ct)
            ?? throw new NotFoundException("Post", request.PostId);

        var dtos = post.Likes
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new LikeDto(
                l.LikeId,
                l.UserId,
                l.User?.Username.Value ?? string.Empty,
                l.CreatedAt))
            .ToList();

        return Result.Success<IReadOnlyList<LikeDto>>(dtos);
    }
}
