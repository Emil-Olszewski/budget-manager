using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Accounts;

public record DeleteAccountCommand(int Id) : IRequest;

internal sealed class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand>
{
    private readonly IContext context;

    public DeleteAccountCommandHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task Handle(DeleteAccountCommand request, CancellationToken ct)
    {
        var account = await context.Set<Account>()
            .Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (account is null)
        {
            throw new BusinessException("Account was not found");
        }
        
        context.Remove(account);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}