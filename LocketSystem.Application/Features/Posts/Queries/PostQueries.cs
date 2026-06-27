using FluentValidation;
using LocketMini.Application.Common;
using LocketMini.Application.DTOs;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketMini.Application.Features.Posts.Queries;

// ── GetFeed ───────────────────────────────────────────────────────────────────

public sealed record GetFeedQuery(
    int RequesterId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<PostDto>>>;

public sealed class GetFeedValidator : AbstractValidator<GetFeedQuery>
{
    public GetFeedValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}

public sealed class GetFeedHandler : IRequestHandler<GetFeedQuery, Result<PagedResult<PostDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetFeedHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedResult<PostDto>>> Handle(GetFeedQuery request, CancellationToken ct)
    {
        // Lấy danh sách bạn bè + bản thân
        var friendIds = await _uow.Friends.GetFriendIdsAsync(request.RequesterId, ct);
        var authorIds = friendIds.Concat(new[] { request.RequesterId }).ToList();

        var posts = await _uow.Posts.GetFeedAsync(authorIds, request.Page, request.PageSize, ct);

        var dtos = posts.Select(p => MapToDto(p, request.RequesterId)).ToList();

        // Tổng count để phân trang (đơn giản: nếu trả về đủ pageSize thì còn trang tiếp)
        var pagedResult = new PagedResult<PostDto>(dtos, request.Page, request.PageSize, dtos.Count);
        return Result.Success(pagedResult);
    }

    private static PostDto MapToDto(Domain.Entities.Post p, int requesterId) => new(
        p.PostId,
        p.UserId,
        p.Owner?.Username.Value ?? string.Empty,
        p.Caption,
        p.ImageUrl,
        p.CreatedAt,
        p.LikeCount,
        p.CommentCount,
        p.Likes.Any(l => l.UserId == requesterId));
}

// ── GetPostDetail ─────────────────────────────────────────────────────────────

public sealed record GetPostDetailQuery(
    int RequesterId,
    int PostId) : IRequest<Result<PostDto>>;

public sealed class GetPostDetailHandler : IRequestHandler<GetPostDetailQuery, Result<PostDto>>
{
    private readonly IUnitOfWork _uow;

    public GetPostDetailHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PostDto>> Handle(GetPostDetailQuery request, CancellationToken ct)
    {
        var post = await _uow.Posts.GetWithDetailsAsync(request.PostId, ct)
            ?? throw new NotFoundException("Post", request.PostId);

        var dto = new PostDto(
            post.PostId,
            post.UserId,
            post.Owner?.Username.Value ?? string.Empty,
            post.Caption,
            post.ImageUrl,
            post.CreatedAt,
            post.LikeCount,
            post.CommentCount,
            post.Likes.Any(l => l.UserId == request.RequesterId));

        return Result.Success(dto);
    }
}

// ── GetUserPosts ──────────────────────────────────────────────────────────────

public sealed record GetUserPostsQuery(
    int RequesterId,
    int TargetUserId) : IRequest<Result<IReadOnlyList<PostDto>>>;

public sealed class GetUserPostsHandler : IRequestHandler<GetUserPostsQuery, Result<IReadOnlyList<PostDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetUserPostsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetUserPostsQuery request, CancellationToken ct)
    {
        var posts = await _uow.Posts.GetByUserAsync(request.TargetUserId, ct);

        var dtos = posts.Select(p => new PostDto(
            p.PostId,
            p.UserId,
            p.Owner?.Username.Value ?? string.Empty,
            p.Caption,
            p.ImageUrl,
            p.CreatedAt,
            p.LikeCount,
            p.CommentCount,
            p.Likes.Any(l => l.UserId == request.RequesterId)))
            .ToList();

        return Result.Success<IReadOnlyList<PostDto>>(dtos);
    }
}

// ── GetPostStats (thống kê tổng hợp như câu SQL mẫu) ─────────────────────────

public sealed record GetPostStatsQuery(int PostId) : IRequest<Result<PostStatsDto>>;

public sealed class GetPostStatsHandler : IRequestHandler<GetPostStatsQuery, Result<PostStatsDto>>
{
    private readonly IUnitOfWork _uow;

    public GetPostStatsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PostStatsDto>> Handle(GetPostStatsQuery request, CancellationToken ct)
    {
        var post = await _uow.Posts.GetWithDetailsAsync(request.PostId, ct)
            ?? throw new NotFoundException("Post", request.PostId);

        var dto = new PostStatsDto(
            post.PostId,
            post.Owner?.Username.Value ?? string.Empty,
            post.Caption,
            post.ImageUrl,
            post.LikeCount,
            post.CommentCount);

        return Result.Success(dto);
    }
}

