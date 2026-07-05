using LocketMini.Application.Common;
using LocketMini.Application.DTOs;
using LocketMini.Domain.Entities;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketMini.Application.Features.Friends.Queries;

// ── GetFriendList (chỉ những người đã Accepted) ──────────────────────────────

public sealed record GetFriendListQuery(int UserId) : IRequest<Result<IReadOnlyList<FriendDto>>>;

public sealed class GetFriendListHandler
    : IRequestHandler<GetFriendListQuery, Result<IReadOnlyList<FriendDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetFriendListHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IReadOnlyList<FriendDto>>> Handle(
        GetFriendListQuery request, CancellationToken ct)
    {
        // Đảm bảo user tồn tại
        _ = await _uow.Users.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);

        var friendIds = await _uow.Friends.GetFriendIdsAsync(request.UserId, ct);

        // Load từng user để lấy profile
        var friendDtos = new List<FriendDto>();
        foreach (var fid in friendIds)
        {
            var friendUser = await _uow.Users.GetWithProfileAsync(fid, ct);
            if (friendUser is null) continue;

            friendDtos.Add(new FriendDto(
                friendUser.UserId,
                friendUser.Username.Value,
                friendUser.Profile?.FullName));
        }

        return Result.Success<IReadOnlyList<FriendDto>>(friendDtos);
    }
}

// ── GetIncomingFriendRequests (lời mời tôi ĐÃ NHẬN, đang chờ) ────────────────

public sealed record GetIncomingFriendRequestsQuery(int UserId)
    : IRequest<Result<IReadOnlyList<FriendRequestDto>>>;

public sealed class GetIncomingFriendRequestsHandler
    : IRequestHandler<GetIncomingFriendRequestsQuery, Result<IReadOnlyList<FriendRequestDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetIncomingFriendRequestsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IReadOnlyList<FriendRequestDto>>> Handle(
        GetIncomingFriendRequestsQuery request, CancellationToken ct)
    {
        var rows = await _uow.Friends.GetIncomingRequestsAsync(request.UserId, ct);

        var dtos = rows
            .Select(f => new FriendRequestDto(
                f.User.UserId,
                f.User.Username.Value,
                f.User.Profile?.FullName,
                f.CreatedAt))
            .ToList();

        return Result.Success<IReadOnlyList<FriendRequestDto>>(dtos);
    }
}

// ── GetOutgoingFriendRequests (lời mời tôi ĐÃ GỬI, đang chờ) ─────────────────

public sealed record GetOutgoingFriendRequestsQuery(int UserId)
    : IRequest<Result<IReadOnlyList<FriendRequestDto>>>;

public sealed class GetOutgoingFriendRequestsHandler
    : IRequestHandler<GetOutgoingFriendRequestsQuery, Result<IReadOnlyList<FriendRequestDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetOutgoingFriendRequestsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IReadOnlyList<FriendRequestDto>>> Handle(
        GetOutgoingFriendRequestsQuery request, CancellationToken ct)
    {
        var rows = await _uow.Friends.GetOutgoingRequestsAsync(request.UserId, ct);

        var dtos = rows
            .Select(f => new FriendRequestDto(
                f.FriendUser.UserId,
                f.FriendUser.Username.Value,
                f.FriendUser.Profile?.FullName,
                f.CreatedAt))
            .ToList();

        return Result.Success<IReadOnlyList<FriendRequestDto>>(dtos);
    }
}

// ── GetFriendshipStatus (trạng thái quan hệ giữa 2 user, dùng cho trang cá nhân) ──

public sealed record GetFriendshipStatusQuery(
    int MyUserId,
    int OtherUserId) : IRequest<Result<FriendshipRelation>>;

public sealed class GetFriendshipStatusHandler
    : IRequestHandler<GetFriendshipStatusQuery, Result<FriendshipRelation>>
{
    private readonly IUnitOfWork _uow;

    public GetFriendshipStatusHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<FriendshipRelation>> Handle(
        GetFriendshipStatusQuery request, CancellationToken ct)
    {
        if (request.MyUserId == request.OtherUserId)
            return Result.Success(FriendshipRelation.None);

        // Dòng do "tôi" sở hữu, hướng tới người kia
        var mine = await _uow.Friends.GetDirectedAsync(request.MyUserId, request.OtherUserId, ct);
        // Dòng do người kia sở hữu, hướng tới "tôi"
        var theirs = await _uow.Friends.GetDirectedAsync(request.OtherUserId, request.MyUserId, ct);

        var relation =
            (mine?.Status == FriendStatus.Accepted || theirs?.Status == FriendStatus.Accepted)
                ? FriendshipRelation.Friends
            : mine?.Status == FriendStatus.Pending
                ? FriendshipRelation.PendingSentByMe
            : theirs?.Status == FriendStatus.Pending
                ? FriendshipRelation.PendingReceivedByMe
            : FriendshipRelation.None;

        return Result.Success(relation);
    }
}

// ── GetMutualFriends ──────────────────────────────────────────────────────────

public sealed record GetMutualFriendsQuery(
    int UserId,
    int OtherUserId) : IRequest<Result<IReadOnlyList<FriendDto>>>;

public sealed class GetMutualFriendsHandler
    : IRequestHandler<GetMutualFriendsQuery, Result<IReadOnlyList<FriendDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetMutualFriendsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IReadOnlyList<FriendDto>>> Handle(
        GetMutualFriendsQuery request, CancellationToken ct)
    {
        var myFriendIds = await _uow.Friends.GetFriendIdsAsync(request.UserId, ct);
        var otherFriendIds = await _uow.Friends.GetFriendIdsAsync(request.OtherUserId, ct);

        var mutualIds = myFriendIds.Intersect(otherFriendIds).ToList();

        var dtos = new List<FriendDto>();
        foreach (var mid in mutualIds)
        {
            var u = await _uow.Users.GetWithProfileAsync(mid, ct);
            if (u is null) continue;
            dtos.Add(new FriendDto(u.UserId, u.Username.Value, u.Profile?.FullName));
        }

        return Result.Success<IReadOnlyList<FriendDto>>(dtos);
    }
}