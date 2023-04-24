using Core.Application.Features.Accounts;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Accounts;

[TestFixture]
internal sealed class UpdateAccountTests : TestBase
{
    private UpdateAccountCommandHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new UpdateAccountCommandHandler(Context);
    }

    [Test]
    public async Task Handle_AccountNotExists_BusinessException()
    {
        // Arrange
        var command = new UpdateAccountCommand(1, "Name", Currency.Pln);
        
        // Act && Assert
        var action = async () => await handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }
    
    [Test]
    [TestCase("")]
    [TestCase("12345123451234512345123451234512345123451234512345123451234512345")]
    public async Task Handle_NameIsInvalid_BusinessException(string name)
    {
        // Arrange
        var account = Account.Create("Name");
        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var command = new UpdateAccountCommand(1, name, Currency.Pln);

        // Act && Assert
        var action = async () => await handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_AccountUpdated()
    {
        // Arrange
        var account = Account.Create("Name");
        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var command = new UpdateAccountCommand(1, "Test name", Currency.Pln);

        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Context.Accounts.ToList().Should().ContainSingle(x => x.Name == "Test name");
    }
    
    [Test]
    public async Task Handle_InitialBalanceSet_AccountUpdated()
    {
        // Arrange
        var account = Account.Create("Name");
        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var command = new UpdateAccountCommand(1, "Test name", Currency.Pln, 15.0M);

        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Context.Accounts.ToList().Should().ContainSingle(x => x.Name == "Test name");
        Context.Transactions.ToList().Should().ContainSingle(x => x.TransactionDetails.Amount == 15.0M);
    }
    
    [Test]
    public async Task Handle_InitialBalanceChanged_AccountUpdated()
    {
        // Arrange
        var account = Account.Create("Name");
        account.SetInitialBalance(10.0M);
        Context.Add(account);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
        
        var command = new UpdateAccountCommand(1, "Test name", Currency.Pln, 15.0M);

        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Context.Accounts.ToList().Should().ContainSingle(x => x.Name == "Test name");
        Context.Transactions.ToList().Should().ContainSingle(x => x.TransactionDetails.Amount == 15.0M);
        Context.Transactions.Should().HaveCount(1);
    }
    
    [Test]
    public async Task Handle_RemovedInitialBalance_AccountUpdated()
    {
        // Arrange
        var account = Account.Create("Name");
        account.SetInitialBalance(10.0M);
        Context.Add(account);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
        
        var command = new UpdateAccountCommand(1, "Test name", Currency.Pln, 0.0M);

        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Context.Accounts.ToList().Should().ContainSingle(x => x.Name == "Test name");
        Context.Transactions.Should().HaveCount(0);
    }
}