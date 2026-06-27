using LocketMini.Application.Common;
using LocketMini.Application.DTOs;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketMini.Application.Features.Friends.Queries;

// ── GetFriendList ─────────────────────────────────────────────────────────────

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
