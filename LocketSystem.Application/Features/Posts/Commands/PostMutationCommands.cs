using LocketMini.Application.Common;
using LocketMini.Application.Common.Exceptions;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketMini.Application.Features.Posts.Commands;

// ── Delete Post ───────────────────────────────────────────────────────────────

public sealed record DeletePostCommand(
    int RequesterId,
    int PostId) : IRequest<Result>;

public sealed class DeletePostHandler : IRequestHandler<DeletePostCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeletePostHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(DeletePostCommand request, CancellationToken ct)
    {
        var post = await _uow.Posts.GetByIdAsync(request.PostId, ct)
            ?? throw new NotFoundException("Post", request.PostId);

        if (post.UserId != request.RequesterId)
            throw new ForbiddenException("Bạn không có quyền xóa bài viết này.");

        _uow.Posts.Remove(post);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── Update Caption ────────────────────────────────────────────────────────────

public sealed record UpdatePostCaptionCommand(
    int RequesterId,
    int PostId,
    string? Caption) : IRequest<Result>;

public sealed class UpdatePostCaptionHandler : IRequestHandler<UpdatePostCaptionCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public UpdatePostCaptionHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(UpdatePostCaptionCommand request, CancellationToken ct)
    {
        var post = await _uow.Posts.GetByIdAsync(request.PostId, ct)
            ?? throw new NotFoundException("Post", request.PostId);

        if (post.UserId != request.RequesterId)
            throw new ForbiddenException("Bạn không có quyền chỉnh sửa bài viết này.");

        post.UpdateCaption(request.Caption);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
