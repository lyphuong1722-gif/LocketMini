using FluentValidation;
using LocketMini.Application.Common;
using LocketMini.Application.DTOs;
using LocketMini.Domain.Interfaces.Repositories;
using MediatR;

namespace LocketMini.Application.Features.Users.Queries;

// ── SearchUsers ───────────────────────────────────────────────────────────────

public sealed record SearchUsersQuery(
    string Keyword,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<UserSummaryDto>>>;

public sealed class SearchUsersValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersValidator()
    {
        RuleFor(x => x.Keyword)
            .NotEmpty().WithMessage("Từ khóa tìm kiếm không được trống.")
            .MinimumLength(2).WithMessage("Từ khóa phải có ít nhất 2 ký tự.")
            .MaximumLength(50);

        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}

public sealed class SearchUsersHandler
    : IRequestHandler<SearchUsersQuery, Result<PagedResult<UserSummaryDto>>>
{
    private readonly IUserRepository _users;

    public SearchUsersHandler(IUserRepository users) => _users = users;

    public async Task<Result<PagedResult<UserSummaryDto>>> Handle(
        SearchUsersQuery request, CancellationToken ct)
    {
        var allMatches = await _users.SearchAsync(request.Keyword, ct);

        var total = allMatches.Count;
        var paged = allMatches
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserSummaryDto(
                u.UserId,
                u.Username.Value,
                u.Profile?.FullName))
            .ToList();

        return Result.Success(new PagedResult<UserSummaryDto>(paged, request.Page, request.PageSize, total));
    }
}
