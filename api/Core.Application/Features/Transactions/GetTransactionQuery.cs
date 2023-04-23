using Core.Application.Features.Tags;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Transactions;

public sealed record GetTransactionQuery(int Id) : IRequest<TransactionResponse>;

internal sealed class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionResponse>
{
    private readonly IContext context;

    public GetTransactionQueryHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task<TransactionResponse> Handle(GetTransactionQuery request, CancellationToken ct)
    {
        var transaction = await context.Set<Transaction>()
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Include(x => x.Account)
            .Include(x => x.Tags).ThenInclude(x => x.Parent)
            .Select(x => TransactionResponse.MapFrom(x))
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);
        
        if (transaction is null)
        {
            throw new BusinessException("Transaction was not found");
        }

        return transaction;
    }
}
