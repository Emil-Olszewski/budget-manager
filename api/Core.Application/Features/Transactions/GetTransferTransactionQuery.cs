using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Transactions;

internal sealed record TransferTransactionAccountResponse(int Id, string Name);

internal sealed record TransferTransactionTransactionResponse(int Id, decimal Amount);

internal sealed class TransferTransactionResponse
{
    public int Id { get; set; }
    public TransferTransactionAccountResponse AccountFrom { get; set; }
    public TransferTransactionAccountResponse AccountTo { get; set; }
    public TransferTransactionTransactionResponse InputTransaction { get; set; }
    public TransferTransactionTransactionResponse OutputTransaction { get; set; }
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
            Id = transfer.Id,
            AccountFrom = new TransferTransactionAccountResponse(transfer.Input.Account.Id, transfer.Input.Account.Name),
            AccountTo = new TransferTransactionAccountResponse(transfer.Output.Account.Id, transfer.Output.Account.Name),
            InputTransaction = new TransferTransactionTransactionResponse(transfer.Input.Id, transfer.Input.TransactionDetails.Amount),
            OutputTransaction = new TransferTransactionTransactionResponse(transfer.Output.Id, transfer.Output.TransactionDetails.Amount),
            CurrencyConversionRate = transfer.CurrencyConversionRate,
            Date = transfer.Input.Date,
        };
    }
} 