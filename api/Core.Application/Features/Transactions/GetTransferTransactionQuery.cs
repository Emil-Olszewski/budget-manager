using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Transactions;

internal sealed class TransferTransactionAccountResponse
{
    public int Id { get; set; }

    public string Name { get; set; }

    public TransferTransactionAccountResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

internal sealed class TransferTransactionResponse
{
    public TransferTransactionAccountResponse AccountFrom { get; set; }
    public TransferTransactionAccountResponse AccountTo { get; set; }
    public decimal InputAmount { get; set; }
    public decimal OutputAmount { get; set; }
    public decimal? CurrencyConversionRate { get; set; }
    public DateTime Date { get; set; }
}

public sealed record GetTransferTransactionQuery(int Id) : IRequest<TransferTransactionResponse>;


internal sealed class GetTransferTransactionQueryHandler : IRequestHandler<GetTransferTransactionQuery, TransferTransactionResponse>
{
    private readonly IContext context;

    public GetTransferTransactionQueryHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task<TransferTransactionResponse> Handle(GetTransferTransactionQuery request, CancellationToken ct)
    {
        var transfer = await context.Set<TransferTransaction>()
            .AsNoTracking()
            .Include(x => x.Input).ThenInclude(x => x.Account)
            .Include(x => x.Output).ThenInclude(x => x.Account)
            .Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (transfer is null)
        {
            throw new BusinessException("Transfer does not exists");
        }
        
        return new TransferTransactionResponse
        {
            AccountFrom = new TransferTransactionAccountResponse(transfer.Input.Account.Id, transfer.Input.Account.Name),
            AccountTo = new TransferTransactionAccountResponse(transfer.Output.Account.Id, transfer.Output.Account.Name),
            InputAmount = transfer.Input.TransactionDetails.Amount,
            OutputAmount = transfer.Output.TransactionDetails.Amount,
            CurrencyConversionRate = transfer.CurrencyConversionRate,
            Date = transfer.Input.Date,
        };
    }
} 