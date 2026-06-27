using FluentValidation;
using LocketMini.Application.Common;
using LocketMini.Application.DTOs;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketMini.Application.Features.Posts.Commands;

// ── Command ───────────────────────────────────────────────────────────────────

public sealed record CreatePostCommand(
    int RequesterId,
    string? Caption,
    string? ImageUrl) : IRequest<Result<int>>;

// ── Validator ─────────────────────────────────────────────────────────────────

public sealed class CreatePostValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Caption) || !string.IsNullOrWhiteSpace(x.ImageUrl))
            .WithMessage("Bài viết phải có caption hoặc ảnh.");

        RuleFor(x => x.ImageUrl)
            .Must(url => Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _))
            .When(x => x.ImageUrl is not null)
            .WithMessage("URL ảnh không hợp lệ.");
    }
}

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class CreatePostHandler : IRequestHandler<CreatePostCommand, Result<int>>
{
    private readonly IUnitOfWork _uow;

    public CreatePostHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<int>> Handle(CreatePostCommand request, CancellationToken ct)
    {
        var user = await _uow.Users.GetByIdAsync(request.RequesterId, ct)
            ?? throw new NotFoundException("User", request.RequesterId);

        var post = user.AddPost(request.Caption, request.ImageUrl);

        await _uow.SaveChangesAsync(ct);
        return Result.Success(post.PostId);
    }
}
