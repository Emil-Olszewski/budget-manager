using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Transactions;

public sealed record UpdateTransactionCommand : IRequest
{
    public int Id { get; init; }
    public int AccountId { get; init; }
    public string Name { get; init; }
    public decimal Amount { get; init; }
    public TransactionType Type { get; init; }
    public DateTime Date { get; init; }
    public List<int> TagIds { get; init; } = new();
}

internal sealed class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand>
{
    private readonly IContext context;

    public UpdateTransactionCommandHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task Handle(UpdateTransactionCommand request, CancellationToken ct)
    {
        var transaction = await context.Set<Transaction>()
            .Where(x => x.Id == request.Id)
            .Include(x => x.Tags)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (transaction is null)
        {
            throw new BusinessException("Transaction do not exists");
        }
        
        var account = await context.Set<Account>()
            .Where(x => x.Id == request.AccountId)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (account is null)
        {
            throw new BusinessException("Account does not exists");
        }

        transaction.Account = account;
        transaction.Name = request.Name;
        transaction.Date = request.Date;
        transaction.TransactionDetails = new TransactionDetails(request.Amount, request.Type);

        var transactionTagIds = transaction.Tags.Select(x => x.Id).ToList();
        
        var tagIdsToAdd = request.TagIds.Except(transactionTagIds).ToList();
        if (tagIdsToAdd.Any())
        {
            var tagsToAdd = await context.Set<Tag>().Where(x => tagIdsToAdd.Contains(x.Id))
                .ToListAsync(ct)
                .ConfigureAwait(false);

            if (tagsToAdd.Count != tagIdsToAdd.Count)
            {
                throw new BusinessException("One or more tags do not exist");
            }

            foreach (var tag in tagsToAdd)
            {
                transaction.AddTag(tag);
            }
        }
        
        var tagsToRemove = transaction.Tags
            .Where(x => transactionTagIds.Except(request.TagIds).Contains(x.Id))
            .ToList();
        
        foreach (var tag in tagsToRemove)
        {
            transaction.RemoveTag(tag);
        }
        
        await context.SaveChangesAsync(ct);
    }
}