using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Transactions;

public sealed record UpdateTransferTransactionCommand : IRequest
{
    public int Id { get; init; }
    public decimal Amount { get; init; }
    public decimal? CurrencyConversionRate { get; init; }
    public DateTime Date { get; init; }
}

internal sealed class UpdateTransferTransactionCommandHandler : IRequestHandler<UpdateTransferTransactionCommand>
{
    private readonly IContext context;

    public UpdateTransferTransactionCommandHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task Handle(UpdateTransferTransactionCommand request, CancellationToken ct)
    {
        var transfer = await context.Set<TransferTransaction>()
            .Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (transfer is null)
        {
            throw new BusinessException("Transfer does not exists");
        }
        
        transfer.SetAmount(request.Amount, request.CurrencyConversionRate);
        transfer.SetDate(request.Date);

        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}