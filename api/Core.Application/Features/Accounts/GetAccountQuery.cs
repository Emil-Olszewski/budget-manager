using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Accounts;

internal sealed class AccountWithInitialBalanceResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal? InitialBalance { get; set; }
    public Currency Currency { get; set; }
}

public record GetAccountQuery(int Id) : IRequest<AccountWithInitialBalanceResponse>;

internal sealed class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountWithInitialBalanceResponse>
{
    private readonly IContext context;

    public GetAccountQueryHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task<AccountWithInitialBalanceResponse> Handle(GetAccountQuery request, CancellationToken ct)
    {
        var account = await context.Set<Account>()
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new AccountWithInitialBalanceResponse
            {
                Id = x.Id,
                Name = x.Name,
                InitialBalance = 
                    x.Transactions.Any(y => y.TransactionDetails.TransactionType == TransactionType.InitialBalance) ?
                        x.Transactions.First(y => y.TransactionDetails.TransactionType == TransactionType.InitialBalance).TransactionDetails.Amount
                        : null,
                Currency = x.Currency
            })
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (account is null)
        {
            throw new BusinessException("Account was not found");
        }

        return account;
    }
}