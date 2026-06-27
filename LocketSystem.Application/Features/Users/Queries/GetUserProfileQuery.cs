using LocketMini.Application.Common;
using LocketMini.Application.DTOs;
using LocketMini.Domain.Exceptions;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketMini.Application.Features.Users.Queries;

// ── Query ─────────────────────────────────────────────────────────────────────

public sealed record GetUserProfileQuery(int UserId) : IRequest<Result<UserDto>>;

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, Result<UserDto>>
{
    private readonly IUnitOfWork _uow;

    public GetUserProfileHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<UserDto>> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var user = await _uow.Users.GetWithProfileAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);

        var dto = new UserDto(
            user.UserId,
            user.Username.Value,
            user.Profile?.FullName,
            user.Profile?.Bio);

        return Result.Success(dto);
    }
}
