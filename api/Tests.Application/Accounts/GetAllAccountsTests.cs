using Core.Application.Features.Accounts;
using Core.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Accounts;

[TestFixture]
internal sealed class GetAllAccountsTests : TestBase
{
    private GetAllAccountsQueryHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new GetAllAccountsQueryHandler(Context);
    }

    [Test]
    public async Task Handle_AccountsReturned()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create("Account1"),
            Account.Create("Account2")
        };
        
        Context.AddRange(accounts);
        await Context.SaveChangesAsync();

        var request = new GetAllAccountsQuery();
        
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        
        // Arrange
        result.Should().HaveSameCount(accounts);
    }
    
    [Test]
    public async Task Handle_WithInitialBalance_AccountsReturned()
    {
        // Arrange
        var account = Account.Create("Account1");
        account.SetInitialBalance(15.0M);
        
        Context.Add(account);
        await Context.SaveChangesAsync();

        var request = new GetAllAccountsQuery();
        
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        
        // Arrange
        result.Should().ContainSingle(x => x.Balance == 15.0M);
    }

    [Test]
    public async Task Handle_DatesProvided_BalanceCountedInRange()
    {
        // this tests check if balance is counted correctly if we provide dates to filter transactions
        // Arrange
        var account = Account.Create("Account1");
        
        var transactionDetails1 = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction1 = Transaction.Create(account, "Name", transactionDetails1, new DateTime(2023, 01, 14));
        
        var transactionDetails2 = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction2 = Transaction.Create(account, "Name", transactionDetails2, new DateTime(2023, 01, 15));
        
        var transactionDetails3 = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction3 = Transaction.Create(account, "Name", transactionDetails3, new DateTime(2023, 02, 15));
        
        var transactionDetails4 = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction4 = Transaction.Create(account, "Name", transactionDetails4, new DateTime(2023, 02, 16));
        
        Context.AddRange(transaction1, transaction2, transaction3, transaction4);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
       
        var request = new GetAllAccountsQuery
        {
            From = new DateTime(2023, 01, 15),
            To = new DateTime(2023, 02, 15)
        };
        
        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.First().Balance.Should().Be(-20);
    }
}