using FluentValidation;
using LocketMini.Application.Common;
using LocketMini.Application.Common.Interfaces;
using LocketMini.Domain.Entities;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces;
using LocketMini.Domain.Interfaces.Repositories;
using LocketMini.Domain.ValueObjects;
using MediatR;

namespace LocketMini.Application.Features.Auth.Commands;

public sealed record ChangePasswordCommand(
    int RequesterId,
    string CurrentPassword,
    string NewPassword) : IRequest<Result>;

public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Vui lòng nhập mật khẩu hiện tại.");
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6).WithMessage("Mật khẩu mới phải có ít nhất 6 ký tự.")
            .NotEqual(x => x.CurrentPassword).WithMessage("Mật khẩu mới phải khác mật khẩu cũ.");
    }
}

public sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;

    public ChangePasswordHandler(IUnitOfWork uow, IPasswordHasher hasher)
    {
        _uow = uow;
        _hasher = hasher;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await _uow.Users.GetByIdAsync(request.RequesterId, ct)
            ?? throw new NotFoundException(nameof(User), request.RequesterId);

        if (!_hasher.Verify(request.CurrentPassword, user.Password.HashedValue))
            return Result.Failure("Mật khẩu hiện tại không đúng.");

        var newPassword = Password.Create(_hasher.Hash(request.NewPassword));
        user.ChangePassword(newPassword);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}
