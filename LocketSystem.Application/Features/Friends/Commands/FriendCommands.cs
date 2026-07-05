using LocketMini.Application.Common;
using LocketMini.Application.Common.Exceptions;
using LocketMini.Domain.Entities;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketMini.Application.Features.Friends.Commands;

// ── SendFriendRequest ─────────────────────────────────────────────────────────

public sealed record SendFriendRequestCommand(
    int RequesterId,
    int TargetUserId) : IRequest<Result>;

public sealed class SendFriendRequestHandler : IRequestHandler<SendFriendRequestCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public SendFriendRequestHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(SendFriendRequestCommand request, CancellationToken ct)
    {
        if (request.RequesterId == request.TargetUserId)
            return Result.Failure("Không thể gửi lời mời kết bạn cho chính mình.");

        var requester = await _uow.Users.GetWithFriendsAsync(request.RequesterId, ct)
            ?? throw new NotFoundException("User", request.RequesterId);

        var target = await _uow.Users.GetWithFriendsAsync(request.TargetUserId, ct)
            ?? throw new NotFoundException("User", request.TargetUserId);

        try
        {
            // Nếu đối phương đã gửi lời mời cho mình trước đó -> tự động chấp nhận
            // thay vì tạo thêm một lời mời song song.
            var reverseRequestExists = target.Friends
                .Any(f => f.FriendId == request.RequesterId && f.Status == FriendStatus.Pending);

            if (reverseRequestExists)
            {
                target.MarkRequestAccepted(request.RequesterId);
                requester.AddAcceptedFriend(request.TargetUserId);
            }
            else
            {
                requester.SendFriendRequest(request.TargetUserId);
            }

            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}

// ── AcceptFriendRequest ───────────────────────────────────────────────────────

public sealed record AcceptFriendRequestCommand(
    int RequesterId,
    int AccepterId) : IRequest<Result>;

public sealed class AcceptFriendRequestHandler : IRequestHandler<AcceptFriendRequestCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public AcceptFriendRequestHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(AcceptFriendRequestCommand request, CancellationToken ct)
    {
        var requester = await _uow.Users.GetWithFriendsAsync(request.RequesterId, ct)
            ?? throw new NotFoundException("User", request.RequesterId);

        var accepter = await _uow.Users.GetWithFriendsAsync(request.AccepterId, ct)
            ?? throw new NotFoundException("User", request.AccepterId);

        try
        {
            requester.MarkRequestAccepted(request.AccepterId);
            accepter.AddAcceptedFriend(request.RequesterId);

            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}

// ── DeclineFriendRequest (người nhận từ chối lời mời) ────────────────────────

public sealed record DeclineFriendRequestCommand(
    int RequesterId,
    int TargetUserId) : IRequest<Result>;

public sealed class DeclineFriendRequestHandler : IRequestHandler<DeclineFriendRequestCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeclineFriendRequestHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(DeclineFriendRequestCommand request, CancellationToken ct)
    {
        var requester = await _uow.Users.GetWithFriendsAsync(request.RequesterId, ct)
            ?? throw new NotFoundException("User", request.RequesterId);

        try
        {
            requester.DeclineOutgoingRequest(request.TargetUserId);
            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}

// ── CancelFriendRequest (người gửi tự hủy lời mời đã gửi) ────────────────────

public sealed record CancelFriendRequestCommand(
    int RequesterId,
    int TargetUserId) : IRequest<Result>;

public sealed class CancelFriendRequestHandler : IRequestHandler<CancelFriendRequestCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public CancelFriendRequestHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(CancelFriendRequestCommand request, CancellationToken ct)
    {
        var requester = await _uow.Users.GetWithFriendsAsync(request.RequesterId, ct)
            ?? throw new NotFoundException("User", request.RequesterId);

        try
        {
            requester.CancelSentRequest(request.TargetUserId);
            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}

// ── RemoveFriend (hủy kết bạn đã Accepted) ───────────────────────────────────

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

        var target = await _uow.Users.GetWithFriendsAsync(request.TargetUserId, ct)
            ?? throw new NotFoundException("User", request.TargetUserId);

        try
        {
            // Xóa cả hai chiều của mối quan hệ Accepted
            requester.RemoveFriend(request.TargetUserId);
            target.RemoveFriend(request.RequesterId);

            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}