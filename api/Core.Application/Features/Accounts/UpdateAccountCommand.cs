using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Accounts;

public record UpdateAccountCommand(int Id, string Name, Currency Currency, decimal? InitialBalance = null) : IRequest;

internal sealed class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand>
{
    private readonly IContext context;

    public UpdateAccountCommandHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task Handle(UpdateAccountCommand request, CancellationToken ct)
    {
        var account = await context.Set<Account>()
            .Include(x => x.Transactions.Where(x => x.TransactionDetails.TransactionType == TransactionType.InitialBalance))
            .Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (account is null)
        {
            throw new BusinessException("Account was not found");
        }

        account.Name = request.Name;
        account.Currency = request.Currency;

        if (request.InitialBalance is not null)
        {
            account.SetInitialBalance(request.InitialBalance.Value);
        }
        
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}