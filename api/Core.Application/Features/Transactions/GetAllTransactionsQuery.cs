using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Transactions;

public sealed record GetAllTransactionsQuery : IRequest<List<TransactionResponse>>
{
    public DateTime From { get; init; }
    public DateTime To { get; init; }
}

internal sealed class GetAllTransactionQueryHandler : IRequestHandler<GetAllTransactionsQuery, List<TransactionResponse>>
{
    private readonly IContext context;

    public GetAllTransactionQueryHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task<List<TransactionResponse>> Handle(GetAllTransactionsQuery request, CancellationToken ct)
    {
        var allTransactions = await context.Set<Transaction>()
            .AsNoTracking()
            .Include(x => x.Account)
            .Include(x => x.Tags).ThenInclude(x => x.Parent)
            .OrderByDescending(x => x.Date)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var mappedTransactions = allTransactions.Select(TransactionResponse.MapFrom).ToList();

        var groupedTransactions = mappedTransactions.GroupBy(x => x.AccountId).ToList();
        foreach (var groupedTransaction in groupedTransactions)
        {
            groupedTransaction.Last().AccountBalanceAfter = groupedTransaction.Last().Amount;
            for (int i = groupedTransaction.Count()-2; i >= 0; i--)
            {
                // select 4th index of groupdTransaction
                
                groupedTransaction.ElementAt(i).AccountBalanceAfter = groupedTransaction.ElementAt(i+1).AccountBalanceAfter + groupedTransaction.ElementAt(i).Amount;
            }
        }

        mappedTransactions = mappedTransactions.Where(x => x.Type != TransactionType.InitialBalance).ToList();
        
        if (request.From != default)
        {
            mappedTransactions = mappedTransactions.Where(x => x.Date >= request.From).ToList();
        }
        
        if (request.To != default)
        {
            mappedTransactions = mappedTransactions.Where(x => x.Date <= request.To).ToList();
        }

        return mappedTransactions;
    }
}

