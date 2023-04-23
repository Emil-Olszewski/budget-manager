using Core.Application.Features.Transactions;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Transactions;

[TestFixture]
internal sealed class DeleteTransactionTests : TestBase
{
    private DeleteTransactionCommandHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new DeleteTransactionCommandHandler(Context);
    }

    [Test]
    public async Task Handle_TransactionNotExists_BusinessException()
    {
        // Arrange
        var command = new DeleteTransactionCommand(1);
        
        // Act && Assert
        var action = async () => await handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }
    
    [Test]
    public async Task Handle_TransactionDeleted()
    {
        // Arrange
        var account = Account.Create("Name");
        var transactionDetails = new TransactionDetails(10.0m, TransactionType.Income);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        Context.Add(transaction);
        
        await Context.SaveChangesAsync();
        
        var command = new DeleteTransactionCommand(1);
        
        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Context.Transactions.Count().Should().Be(0);
    }

    [Test]
    public async Task Handle_TransferTransaction_DeletedWithCorrespondingTransaction()
    {
        Assert.Fail();
    }
}