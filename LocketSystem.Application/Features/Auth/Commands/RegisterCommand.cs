using FluentValidation;
using LocketMini.Application.Common;
using LocketMini.Application.Common.Interfaces;
using LocketMini.Application.DTOs;
using LocketMini.Domain.Entities;
using LocketMini.Domain.Interfaces;
using LocketMini.Domain.Interfaces.Repositories;
using LocketSystem.Application.Common.Interfaces;
using MediatR;

namespace LocketMini.Application.Features.Auth.Commands;

// ── Command ───────────────────────────────────────────────────────────────────

public sealed record RegisterCommand(
    string Username,
    string Password,
    string? FullName,
    string? Bio) : IRequest<Result<AuthTokenDto>>;

// ── Validator ─────────────────────────────────────────────────────────────────

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Tên đăng nhập không được trống.")
            .MinimumLength(3).WithMessage("Tên đăng nhập phải có ít nhất 3 ký tự.")
            .MaximumLength(50).WithMessage("Tên đăng nhập không được vượt quá 50 ký tự.")
            .Matches(@"^[a-z0-9_]+$").WithMessage("Tên đăng nhập chỉ được chứa chữ thường, số và dấu gạch dưới.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được trống.")
            .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.");
    }
}

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthTokenDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public RegisterCommandHandler(IUnitOfWork uow, IPasswordHasher hasher, IJwtTokenService jwt)
    {
        _uow = uow;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<Result<AuthTokenDto>> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await _uow.Users.ExistsAsync(request.Username, ct))
            return Result.Failure<AuthTokenDto>($"Tên đăng nhập '{request.Username}' đã tồn tại.");

        var hashed = _hasher.Hash(request.Password);
        var user = User.Create(request.Username, hashed);

        if (!string.IsNullOrWhiteSpace(request.FullName) || !string.IsNullOrWhiteSpace(request.Bio))
            user.SetProfile(request.FullName, request.Bio);

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var token = _jwt.GenerateToken(user.UserId, user.Username.Value);
        return Result.Success(new AuthTokenDto(user.UserId, user.Username.Value, token));
    }
}
