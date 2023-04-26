using Core.Application.Features.Transactions;
using Core.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Transactions;

[TestFixture]
internal sealed class GetAllTransactionsTests : TestBase
{
    private GetAllTransactionQueryHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new GetAllTransactionQueryHandler(Context);
    }

    [Test]
    public async Task Handle_TransactionsReturned()
    {
        // Arrange
        var account = Account.Create("Account1");
        var transactionDetails = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        var tag = Tag.Create(null, "Tag", TagType.ForOutcome);
        var childTag = Tag.Create(tag, "Child", TagType.ForOutcome);
        var secondChildTag = Tag.Create(childTag, "Grandchild", TagType.ForOutcome);
        transaction.AddTag(childTag);
        transaction.AddTag(secondChildTag);
        
        Context.Add(transaction);
        await Context.SaveChangesAsync();

        var request = new GetAllTransactionsQuery();
        
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        
        // Arrange
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Name");
        result.First().Amount.Should().Be(transactionDetails.Amount);
        result.First().AccountId.Should().Be(account.Id);
        result.First().Id.Should().Be(transaction.Id);
        result.First().Date.Should().Be(transaction.Date);
        result.First().Type.Should().Be(transactionDetails.TransactionType);
        result.First().Tags.Should().HaveCount(2);
        result.First().Tags.Should().ContainSingle(x => x.Name == "Tag/Child");
        result.First().Tags.Should().ContainSingle(x => x.Name == "Child/Grandchild");
    }

    [Test]
    public async Task Handle_OnlyTransactionWithInitialBalance_NoTransactionsReturned()
    {
        // Arrange
        var account = Account.Create("Account1");
        account.SetInitialBalance(15.0M);

        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var request = new GetAllTransactionsQuery();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        
          
        // Arrange
        result.Should().BeEmpty();
    }

    [Test]
    public async Task Handle_DatesFrovided_FilteredTransactionsReturned()
    {
        // Arrange
        var account = Account.Create("Account1");
        var transactionDetails1 = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transactionDetails2 = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transactionDetails3 = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transactionDetails4 = new TransactionDetails(-10.0m, TransactionType.Expense);
        Context.Add(Transaction.Create(account, "name", transactionDetails1, new DateTime(2023, 01, 14)));
        Context.Add(Transaction.Create(account, "name", transactionDetails2, new DateTime(2023, 01, 15)));
        Context.Add(Transaction.Create(account, "name", transactionDetails3, new DateTime(2023, 02, 15)));
        Context.Add(Transaction.Create(account, "name", transactionDetails4, new DateTime(2023, 02, 16)));
        
        await Context.SaveChangesAsync();
        var request = new GetAllTransactionsQuery
        {
            From = new DateTime(2023, 01, 15),
            To = new DateTime(2023, 02, 15)
        };

   
        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }
}