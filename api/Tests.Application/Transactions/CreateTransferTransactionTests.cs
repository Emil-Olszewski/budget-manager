using Core.Application.Features.Transactions;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Transactions;

internal sealed class CreateTransferTransactionTests : TestBase
{
    private CreateTransferTransactionCommandHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new CreateTransferTransactionCommandHandler(Context);
    }
    
    [Test]
    public async Task Handle_AccountNotExist_BusinessException()
    {
        // Arrange
        var account = Account.Create("Name");
        
        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var request = new CreateTransferTransactionCommand
        {
            AccountFromId = 1,
            AccountToId = 2,
            Amount = 15.0m,
            Date = DateTime.Now,
        };
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }
    
    
    [Test]
    public async Task Handle_TransferCreated()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name");
        
        Context.AddRange(accountFrom, accountTo);
        await Context.SaveChangesAsync();
        
        var request = new CreateTransferTransactionCommand
        {
            AccountFromId = 1,
            AccountToId = 2,
            Amount = 15.0m,
            Date = DateTime.Now,
        };
        
        // Act
        await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Context.Transactions.ToList().Should().HaveCount(2);
        Context.TransferTransactions.ToList().Should().HaveCount(1);
    }
}