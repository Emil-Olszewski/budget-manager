using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Transactions;

public sealed record CreateTransferTransactionCommand : IRequest
{
    public int AccountFromId { get; init; }
    public int AccountToId { get; init; }
    public decimal Amount { get; init; }
    public decimal? CurrencyConversionRate { get; init; }
    public DateTime Date { get; init; }
}

internal sealed class CreateTransferTransactionCommandHandler : IRequestHandler<CreateTransferTransactionCommand>
{
    private readonly IContext context;

    public CreateTransferTransactionCommandHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task Handle(CreateTransferTransactionCommand request, CancellationToken ct)
    {
        var accountFrom = await GetAccount(request.AccountFromId, ct).ConfigureAwait(false);
        var accountTo = await GetAccount(request.AccountToId, ct).ConfigureAwait(false);
        
        var transaction = TransferTransaction.Create(accountFrom, accountTo, request.Date, request.Amount, request.CurrencyConversionRate);

        context.Add(transaction);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private async Task<Account> GetAccount(int accountId, CancellationToken ct)
    {
        var account = await context.Set<Account>()
            .Where(x => x.Id == accountId)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);
        
        if (account is null)
        {
            throw new BusinessException("Account does not exists");
        }

        return account;
    }
}