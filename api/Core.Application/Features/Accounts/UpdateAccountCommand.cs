using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Accounts;

public record UpdateAccountCommand(int Id, string Name, Currency Currency) : IRequest;

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
            .Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (account is null)
        {
            throw new BusinessException("Account was not found");
        }

        account.Name = request.Name;
        account.Currency = request.Currency;
        
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}