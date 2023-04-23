using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Accounts;

internal sealed class AccountResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public Currency Currency { get; set; }
}


public sealed record GetAllAccountsQuery : IRequest<List<AccountResponse>>;

internal sealed class GetAllAccountsQueryHandler : IRequestHandler<GetAllAccountsQuery, List<AccountResponse>>
{
    private readonly IContext context;

    public GetAllAccountsQueryHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task<List<AccountResponse>> Handle(GetAllAccountsQuery request, CancellationToken ct)
    {
        return await context.Set<Account>()
            .AsNoTracking()
            .Select(x => new AccountResponse
            {
                Id = x.Id,
                Name = x.Name,
                Balance = x.Transactions.Sum(y => y.TransactionDetails.Amount),
                Currency = x.Currency
            })
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
}
