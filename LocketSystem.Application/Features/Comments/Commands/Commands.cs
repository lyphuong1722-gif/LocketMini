using FluentValidation;
using LocketMini.Application.Common;
using LocketMini.Application.Common.Exceptions;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketSystem.Application.Features.Comments.Commands;

// ── Add Comment ───────────────────────────────────────────────────────────────

public sealed record AddCommentCommand(
    int RequesterId,
    int PostId,
    string Content) : IRequest<Result<int>>;

public sealed class AddCommentValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Nội dung bình luận không được trống.")
            .MaximumLength(1000).WithMessage("Bình luận không được vượt quá 1000 ký tự.");
    }
}

public sealed class AddCommentHandler : IRequestHandler<AddCommentCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;

    public AddCommentHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<int>> Handle(AddCommentCommand request, CancellationToken ct)
    {
        var post = await _uow.Posts.GetWithDetailsAsync(request.PostId, ct)
            ?? throw new NotFoundException("Post", request.PostId);

        var comment = post.AddComment(request.RequesterId, request.Content);
        await _uow.SaveChangesAsync(ct);
        return Result.Success(comment.CommentId);
    }
}

// ── Delete Comment ────────────────────────────────────────────────────────────

public sealed record DeleteCommentCommand(
    int RequesterId,
    int CommentId) : IRequest<Result>;

public sealed class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeleteCommentHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken ct)
    {
        var comment = await _uow.Comments.GetByIdAsync(request.CommentId, ct)
            ?? throw new NotFoundException("Comment", request.CommentId);

        if (comment.UserId != request.RequesterId)
            throw new ForbiddenException("Bạn không có quyền xóa bình luận này.");

        _uow.Comments.Remove(comment);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── Edit Comment ──────────────────────────────────────────────────────────────

public sealed record EditCommentCommand(
    int RequesterId,
    int CommentId,
    string NewContent) : IRequest<Result>;

public sealed class EditCommentValidator : AbstractValidator<EditCommentCommand>
{
    public EditCommentValidator()
    {
        RuleFor(x => x.NewContent)
            .NotEmpty().WithMessage("Nội dung bình luận không được trống.")
            .MaximumLength(1000).WithMessage("Bình luận không được vượt quá 1000 ký tự.");
    }
}

public sealed class EditCommentHandler : IRequestHandler<EditCommentCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public EditCommentHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(EditCommentCommand request, CancellationToken ct)
    {
        var comment = await _uow.Comments.GetByIdAsync(request.CommentId, ct)
            ?? throw new NotFoundException("Comment", request.CommentId);

        comment.EditContent(request.RequesterId, request.NewContent);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
