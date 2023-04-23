using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Transactions;

public sealed record CreateTransactionCommand : IRequest
{
    public int AccountId { get; init; }
    public string Name { get; init; }
    public decimal Amount { get; init; }
    public TransactionType Type { get; init; }
    public DateTime Date { get; init; }
    public List<int> TagIds { get; init; } = new();
}

internal sealed class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand>
{
    private readonly IContext context;

    public CreateTransactionCommandHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task Handle(CreateTransactionCommand request, CancellationToken ct)
    {
        if (request.Type is TransactionType.InitialBalance or TransactionType.Transfer)
        {
            throw new BusinessException("This type of transaction cannot be created in this method");
        }

        var account = await context.Set<Account>()
            .Where(x => x.Id == request.AccountId)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (account is null)
        {
            throw new BusinessException("Account does not exists");
        }
        
        var transactionDetails = new TransactionDetails(request.Amount, request.Type);
        var transaction = Transaction.Create(account, request.Name, transactionDetails, request.Date);

        if (request.TagIds.Any())
        {
            var tags = await context.Set<Tag>()
                .Where(x => request.TagIds.Contains(x.Id))
                .ToListAsync(ct)
                .ConfigureAwait(false);

            if (tags.Count != request.TagIds.ToHashSet().Count)
            {
                throw new BusinessException("One or more tags do not exist");
            }

            foreach (var tag in tags)
            {
                transaction.AddTag(tag);
            }
        }

        context.Add(transaction);
        await context.SaveChangesAsync(ct);
    }
}