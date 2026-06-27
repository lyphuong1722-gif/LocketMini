using FluentValidation;
using LocketMini.Application.Common;
using LocketMini.Application.Common.Interfaces;
using LocketMini.Application.DTOs;
using LocketMini.Domain.Interfaces;
using LocketMini.Domain.Interfaces.Repositories;
using LocketSystem.Application.Common.Interfaces;
using MediatR;

namespace LocketMini.Application.Features.Auth.Commands;

// ── Command ───────────────────────────────────────────────────────────────────

public sealed record LoginCommand(
    string Username,
    string Password) : IRequest<Result<AuthTokenDto>>;

// ── Validator ─────────────────────────────────────────────────────────────────

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Vui lòng nhập tên đăng nhập.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Vui lòng nhập mật khẩu.");
    }
}

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthTokenDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public LoginCommandHandler(IUnitOfWork uow, IPasswordHasher hasher, IJwtTokenService jwt)
    {
        _uow = uow;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<Result<AuthTokenDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _uow.Users.GetByUsernameAsync(request.Username.ToLower(), ct);

        if (user is null || !_hasher.Verify(request.Password, user.Password.HashedValue))
            return Result.Failure<AuthTokenDto>("Tên đăng nhập hoặc mật khẩu không đúng.");

        var token = _jwt.GenerateToken(user.UserId, user.Username.Value);
        return Result.Success(new AuthTokenDto(user.UserId, user.Username.Value, token));
    }
}
