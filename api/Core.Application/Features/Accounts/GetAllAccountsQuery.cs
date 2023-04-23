using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Accounts;

internal sealed class AccountWithBalanceResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
}

public sealed record GetAllAccountsQuery : IRequest<List<AccountWithBalanceResponse>>;

internal sealed class GetAllAccountsQueryHandler : IRequestHandler<GetAllAccountsQuery, List<AccountWithBalanceResponse>>
{
    private readonly IContext context;

    public GetAllAccountsQueryHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task<List<AccountWithBalanceResponse>> Handle(GetAllAccountsQuery request, CancellationToken ct)
    {
        return await context.Set<Account>()
            .AsNoTracking()
            .Select(x => new AccountWithBalanceResponse
            {
                Id = x.Id,
                Name = x.Name,
                Balance = x.Transactions.Sum(y => y.TransactionDetails.Amount)
            })
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
}
