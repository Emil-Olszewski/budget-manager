using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Transactions;

public sealed record DeleteTransactionCommand(int Id) : IRequest;

internal sealed class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand>
{
    private readonly IContext context;

    public DeleteTransactionCommandHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task Handle(DeleteTransactionCommand request, CancellationToken ct)
    {
        var transaction = await context.Set<Transaction>()
            .Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (transaction is null)
        {
            throw new BusinessException("Transaction do not exists");
        }

        if (transaction.TransactionDetails.TransactionType == TransactionType.Transfer)
        {
            var transfer = await context.Set<TransferTransaction>()
                .Include(x => x.Input)
                .Include(x => x.Output)
                .Where(x => x.Input.Id == request.Id || x.Output.Id == request.Id)
                .SingleAsync(ct)
                .ConfigureAwait(false);

            context.Remove(transfer);
            context.Remove(transfer.Input);
            context.Remove(transfer.Output);
        }
        else
        {
            context.Remove(transaction);
        }

        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}