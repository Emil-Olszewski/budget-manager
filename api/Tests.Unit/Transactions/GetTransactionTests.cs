using Core.Application.Features.Transactions;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Transactions;

[TestFixture]
internal sealed class GetTransactionTests : TestBase
{
    private GetTransactionQueryHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new GetTransactionQueryHandler(Context);
    }

    [Test]
    public async Task Handle_TransactionNotExists_BusinessException()
    {
        // Arrange
        var request = new GetTransactionQuery(1);
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_TransactionReturned()
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

        var request = new GetTransactionQuery(transaction.Id);
        
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        
        // Arrange
        result.Name.Should().Be("Name");
        result.Amount.Should().Be(transactionDetails.Amount);
        result.AccountId.Should().Be(account.Id);
        result.Id.Should().Be(transaction.Id);
        result.Date.Should().Be(transaction.Date);
        result.Type.Should().Be(transactionDetails.TransactionType);
        result.Tags.Should().HaveCount(2);
        result.Tags.Should().ContainSingle(x => x.Name == "Tag/Child");
        result.Tags.Should().ContainSingle(x => x.Name == "Child/Grandchild");
    }
}
