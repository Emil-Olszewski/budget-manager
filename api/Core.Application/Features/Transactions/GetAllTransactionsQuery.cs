using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Transactions;

public sealed record GetAllTransactionsQuery : IRequest<List<TransactionResponse>>;

internal sealed class GetAllTransactionQueryHandler : IRequestHandler<GetAllTransactionsQuery, List<TransactionResponse>>
{
    private readonly IContext context;

    public GetAllTransactionQueryHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task<List<TransactionResponse>> Handle(GetAllTransactionsQuery request, CancellationToken ct)
    {
        return await context.Set<Transaction>()
            .AsNoTracking()
            .Include(x => x.Account)
            .Include(x => x.Tags).ThenInclude(x => x.Parent)
            .Where(x => x.TransactionDetails.TransactionType != TransactionType.InitialBalance)
            .Select(x => TransactionResponse.MapFrom(x))
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
}

