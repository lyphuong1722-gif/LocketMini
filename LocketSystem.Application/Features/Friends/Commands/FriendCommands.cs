using LocketMini.Application.Common;
using LocketMini.Application.Common.Exceptions;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketMini.Application.Features.Friends.Commands;

// ── AddFriend ─────────────────────────────────────────────────────────────────

public sealed record AddFriendCommand(
    int RequesterId,
    int TargetUserId) : IRequest<Result>;

public sealed class AddFriendHandler : IRequestHandler<AddFriendCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public AddFriendHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(AddFriendCommand request, CancellationToken ct)
    {
        if (request.RequesterId == request.TargetUserId)
            return Result.Failure("Không thể kết bạn với chính mình.");

        // Load requester kèm friends để domain check trùng
        var requester = await _uow.Users.GetWithFriendsAsync(request.RequesterId, ct)
            ?? throw new NotFoundException("User", request.RequesterId);

        var targetUser = await _uow.Users.GetByIdAsync(request.TargetUserId, ct)
            ?? throw new NotFoundException("User", request.TargetUserId);

        try
        {
            requester.AddFriend(targetUser);
            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}

// ── RemoveFriend ──────────────────────────────────────────────────────────────

public sealed record RemoveFriendCommand(
    int RequesterId,
    int TargetUserId) : IRequest<Result>;

public sealed class RemoveFriendHandler : IRequestHandler<RemoveFriendCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public RemoveFriendHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(RemoveFriendCommand request, CancellationToken ct)
    {
        var requester = await _uow.Users.GetWithFriendsAsync(request.RequesterId, ct)
            ?? throw new NotFoundException("User", request.RequesterId);

        // Kiểm tra target tồn tại
        var target = await _uow.Users.GetByIdAsync(request.TargetUserId, ct)
            ?? throw new NotFoundException("User", request.TargetUserId);

        // Dùng trực tiếp domain: nếu không tìm thấy friend sẽ throw DomainException
        try
        {
            requester.RemoveFriend(request.TargetUserId);
            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}

// ── CheckFriendship ───────────────────────────────────────────────────────────

public sealed record CheckFriendshipCommand(
    int UserId,
    int OtherUserId) : IRequest<Result<bool>>;

public sealed class CheckFriendshipHandler : IRequestHandler<CheckFriendshipCommand, Result<bool>>
{
    private readonly IUnitOfWork _uow;

    public CheckFriendshipHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<bool>> Handle(CheckFriendshipCommand request, CancellationToken ct)
    {
        var areFriends = await _uow.Friends.AreFriendsAsync(request.UserId, request.OtherUserId, ct);
        return Result.Success(areFriends);
    }
}
