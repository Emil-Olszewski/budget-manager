using Core.Application.Features.Transactions;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Transactions;

internal sealed class UpdateTransferTransactionTests : TestBase
{
    private UpdateTransferTransactionCommandHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new UpdateTransferTransactionCommandHandler(Context);
    }
    
    [Test]
    public async Task Handle_TransferNotExists_BusinessException()
    {
        // Arrange
        var request = new UpdateTransferTransactionCommand()
        {
            Id = 1,
            Amount = 15.0m,
            Date = DateTime.Now,
        };
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }
    
    [Test]
    public async Task Handle_TransferUpdated()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name");
        var transaction = TransferTransaction.Create(accountFrom, accountTo, new DateTime(2000, 1, 1), 15.0m);

        Context.Add(transaction);
        await Context.SaveChangesAsync();

        var dateTime = DateTime.Now;
        var request = new UpdateTransferTransactionCommand()
        {
            Id = 1,
            Amount = 30.0m,
            Date = dateTime,
        };
        
        // Act
        await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Context.Transactions.ToList().Should().HaveCount(2);
        Context.Transactions.Should().ContainSingle(x => x.TransactionDetails.Amount == -30.0m && x.Date == dateTime);
        Context.Transactions.Should().ContainSingle(x => x.TransactionDetails.Amount == 30.0m && x.Date == dateTime);
        Context.TransferTransactions.ToList().Should().HaveCount(1);

    }
}