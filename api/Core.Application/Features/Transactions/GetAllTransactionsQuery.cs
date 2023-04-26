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
        var query = context.Set<Transaction>()
            .AsNoTracking()
            .Include(x => x.Account)
            .Include(x => x.Tags).ThenInclude(x => x.Parent)
            .Where(x => x.TransactionDetails.TransactionType != TransactionType.InitialBalance)
            .AsQueryable();

        if (request.From != default)
        {
            query = query.Where(x => x.Date >= request.From);
        }
        
        if (request.To != default)
        {
            query = query.Where(x => x.Date <= request.To);
        }
        
        var result = await query
            .Select(x => TransactionResponse.MapFrom(x))
            .ToListAsync(ct)
            .ConfigureAwait(false);
        
        return result.OrderByDescending(x => x.Date).ToList();
    }
}

