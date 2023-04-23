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

        context.Remove(transaction);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}