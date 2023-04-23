using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Accounts;

internal sealed record AccountResponse(int Id, string Name);

public record GetAccountQuery(int Id) : IRequest<AccountResponse>;

internal sealed class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountResponse>
{
    private readonly IContext context;

    public GetAccountQueryHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task<AccountResponse> Handle(GetAccountQuery request, CancellationToken ct)
    {
        var account = await context.Set<Account>()
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new AccountResponse(x.Id, x.Name))
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (account is null)
        {
            throw new BusinessException("Account was not found");
        }

        return account;
    }
}