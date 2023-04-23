using Core.Application.Features.Transactions;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Transactions;

[TestFixture]
internal sealed class CreateTransactionTests : TestBase
{
    private CreateTransactionCommandHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new CreateTransactionCommandHandler(Context);
    }

    [Test]
    public async Task Handle_AccountNotExist_BusinessException()
    {
        // Arrange
        var request = new CreateTransactionCommand
        {
            AccountId = 1,
            Name = "Name",
            Amount = -10.0m,
            Date = DateTime.Now,
            Type = TransactionType.Expense
        };
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }
    
    [Test]
    [TestCase("")]
    [TestCase("12345123451234512345123451234512345123451234512345123451234512345")]
    public async Task Handle_NameIsInvalid_BusinessException(string name)
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var request = new CreateTransactionCommand
        {
            AccountId = account.Id,
            Name = name,
            Amount = -10.0m,
            Date = DateTime.Now,
            Type = TransactionType.Expense
        };
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    [TestCase(0.0, TransactionType.Expense)]
    [TestCase(0.0, TransactionType.Income)]
    [TestCase(15.0, TransactionType.Expense)]
    [TestCase(-15.0, TransactionType.Income)]
    public async Task Handle_AmountAndTransactionTypeNotMatching_BusinessException(decimal amount, TransactionType transactionType)
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var request = new CreateTransactionCommand
        {
            AccountId = account.Id,
            Name = "Name",
            Amount = amount,
            Date = DateTime.Now,
            Type = transactionType
        };
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }
    
    [Test]
    public async Task Handle_TooManyTags_BusinessException()
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);
        
        var tags = new List<Tag>
        {
            Tag.Create(null, "tag", TagType.ForOutcome),
            Tag.Create(null, "tag", TagType.ForOutcome),
            Tag.Create(null, "tag", TagType.ForOutcome),
            Tag.Create(null, "tag", TagType.ForOutcome),
            Tag.Create(null, "tag", TagType.ForOutcome),
            Tag.Create(null, "tag", TagType.ForOutcome)
        };
        
        Context.AddRange(tags);
        await Context.SaveChangesAsync();
        
        var request = new CreateTransactionCommand
        {
            AccountId = account.Id,
            Name = "Name",
            Amount = -10.0m,
            Date = DateTime.Now,
            Type = TransactionType.Expense,
            TagIds = tags.Select(x => x.Id).ToList()
        };
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    [TestCase(-10.0,TagType.ForIncome, TransactionType.Expense)]
    [TestCase(10.0,TagType.ForOutcome, TransactionType.Income)]
    public async Task Handle_TagWithInvalidType_BusinessException(decimal amount, TagType tagType, TransactionType transactionType)
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);
        
        Context.Add( Tag.Create(null, "tag", tagType));
        await Context.SaveChangesAsync();
        
        var request = new CreateTransactionCommand
        {
            AccountId = account.Id,
            Name = "Name",
            Amount = amount,
            Date = DateTime.Now,
            Type = transactionType,
            TagIds = new List<int> { 1 }
        };
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }
    
    [Test]
    public async Task Handle_TagNotExists_BusinessException()
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var request = new CreateTransactionCommand
        {
            AccountId = account.Id,
            Name = "Name",
            Amount = -10.0m,
            Date = DateTime.Now,
            Type = TransactionType.Expense,
            TagIds = new List<int> { 1 }
        };
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }
    
    [Test]
    [TestCase(TransactionType.NotDefined)]
    [TestCase(TransactionType.Transfer)]
    [TestCase(TransactionType.InitialBalance)]
    public async Task Handle_WithWrongType_BusinessException(TransactionType transactionType)
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var request = new CreateTransactionCommand
        {
            AccountId = account.Id,
            Name = "Name",
            Amount = 100.0m,
            Date = DateTime.Now,
            Type = transactionType,
        };
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_TransactionCreated()
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var request = new CreateTransactionCommand
        {
            AccountId = account.Id,
            Name = "Name",
            Amount = -100.0m,
            Date = DateTime.Now,
            Type = TransactionType.Expense,
        };
        
        // Act
        await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Context.Transactions.ToList().Should().ContainSingle(x => x.Name == "Name" && x.TransactionDetails.Amount == -100m);
    }
    
    [Test]
    public async Task Handle_TagsProvided_TransactionCreated()
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var tags = new List<Tag>
        {
            Tag.Create(null, "tag", TagType.ForOutcome),
            Tag.Create(null, "tag", TagType.ForOutcome),
            Tag.Create(null, "tag", TagType.ForOutcome)
        };
        
        Context.AddRange(tags);
        await Context.SaveChangesAsync();
        
        var request = new CreateTransactionCommand
        {
            AccountId = account.Id,
            Name = "Name",
            Amount = -100.0m,
            Date = DateTime.Now,
            Type = TransactionType.Expense,
            TagIds = tags.Select(x => x.Id).ToList()
        };
        
        // Act
        await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Context.Transactions.ToList().Should().ContainSingle(x => x.Name == "Name" && x.TransactionDetails.Amount == -100m);
        Context.Transactions.First().Tags.Should().HaveCount(3);
    }
}