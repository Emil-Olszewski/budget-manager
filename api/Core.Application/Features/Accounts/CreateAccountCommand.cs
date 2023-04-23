using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Accounts;

public record CreateAccountCommand(string Name, Currency Currency, decimal? InitialBalance = null) : IRequest;

internal sealed class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand>
{
    private readonly IContext context;

    public CreateAccountCommandHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task Handle(CreateAccountCommand request, CancellationToken ct)
    {
        var account = Account.Create(request.Name, request.Currency);
        if (request.InitialBalance != null)
        {
            account.SetInitialBalance(request.InitialBalance.Value);
        }
        
        context.Add(account);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
