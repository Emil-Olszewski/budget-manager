using Core.Application.Features.Transactions;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Transactions;

[TestFixture]
internal sealed class UpdateTransactionTests : TestBase
{
    private UpdateTransactionCommandHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new UpdateTransactionCommandHandler(Context);
    }

    [Test]
    public async Task Handle_TransactionNotExist_BusinnesException()
    {
        // Arrange
        var request = new UpdateTransactionCommand
        {
            Id = 1,
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
    public async Task Handle_AccountNotExist_BusinessException()
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);
        
        var transactionDetails = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        Context.Add(transaction);
        
        await Context.SaveChangesAsync();
        
        var request = new UpdateTransactionCommand
        {
            Id = transaction.Id,
            AccountId = 2,
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

        var transactionDetails = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        Context.Add(transaction);
        
        await Context.SaveChangesAsync();
        
        var request = new UpdateTransactionCommand
        {
            Id = transaction.Id,
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
        
        var transactionDetails = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        Context.Add(transaction);
        
        await Context.SaveChangesAsync();
        
        var request = new UpdateTransactionCommand
        {
            Id = transaction.Id,
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

        var transactionDetails = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        transaction.AddTag(tags[0]);
        transaction.AddTag(tags[1]);
        transaction.AddTag(tags[2]);
        Context.Add(transaction);
        
        await Context.SaveChangesAsync();
        
        var request = new UpdateTransactionCommand
        {
            Id = transaction.Id,
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
        
        var transactionDetails = new TransactionDetails(amount, transactionType);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        Context.Add(transaction);
        
        Context.Add( Tag.Create(null, "tag", tagType));
        await Context.SaveChangesAsync();
        
        var request = new UpdateTransactionCommand
        {
            Id = transaction.Id,
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
        var account = Account.Create("account");
        Context.Add(account);

        var transactionDetails = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        Context.Add(transaction);
        
        await Context.SaveChangesAsync();

        var request = new UpdateTransactionCommand
        {
            Id = transaction.Id,
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
        var account = Account.Create("account");
        Context.Add(account);

        var transactionDetails = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        Context.Add(transaction);
        
        await Context.SaveChangesAsync();
        var request = new UpdateTransactionCommand
        {
            Id = transaction.Id,
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
    public async Task Handle_TypeModifiedWhenTagsAttached_BusinessException()
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);

        var transactionDetails = new TransactionDetails(100.0m, TransactionType.Income);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        
        var tag = Tag.Create(null, "tag", TagType.ForIncome);
        transaction.AddTag(tag);
        
        Context.Add(transaction);
        
        await Context.SaveChangesAsync();
        
        var request = new UpdateTransactionCommand
        {
            Id = transaction.Id,
            AccountId = account.Id,
            Name = "Name",
            Amount = -100.0m,
            Date = DateTime.Now,
            Type = TransactionType.Expense,
            TagIds = new List<int> { tag.Id }
        };
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_TransactionUpdated()
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);
        
        var transactionDetails = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        Context.Add(transaction);
        
        await Context.SaveChangesAsync(); 
        
        var request = new UpdateTransactionCommand
        {
            Id = transaction.Id,
            AccountId = account.Id,
            Name = "Renamed",
            Amount = -100.0m,
            Date = DateTime.Now,
            Type = TransactionType.Expense,
        };
        
        // Act
        await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Context.Transactions.ToList().Should().ContainSingle(x => x.Name == "Renamed" && x.TransactionDetails.Amount == -100m);
    }
    
    [Test]
    public async Task Handle_TagsModified_TransactionUpdated()
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);
        
        var tags = new List<Tag>
        {
            Tag.Create(null, "tag1", TagType.ForOutcome),
            Tag.Create(null, "tag2", TagType.ForOutcome),
            Tag.Create(null, "tag3", TagType.ForOutcome),
            Tag.Create(null, "tag4", TagType.ForOutcome)
        };
        
        Context.AddRange(tags);

        var transactionDetails = new TransactionDetails(-10.0m, TransactionType.Expense);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        transaction.AddTag(tags[0]);
        transaction.AddTag(tags[1]);
        Context.Add(transaction);
        
        await Context.SaveChangesAsync();

        var newTagIds = new List<int>() { tags[1].Id, tags[2].Id, tags[3].Id };
        var request = new UpdateTransactionCommand
        {
            Id = transaction.Id,
            AccountId = account.Id,
            Name = "Renamed",
            Amount = -100.0m,
            Date = DateTime.Now,
            Type = TransactionType.Expense,
            TagIds = newTagIds
        };
        
        // Act
        await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Context.Transactions.ToList().Should().ContainSingle(x => x.Name == "Renamed" && x.TransactionDetails.Amount == -100m);
        Context.Transactions.First().Tags.Select(x => x.Id).Should().BeEquivalentTo(newTagIds);
    }

    [Test]
    public async Task Handle_TypeModifiedAndTagsDetached_TransactionUpdated()
    {
        // Arrange
        var account = Account.Create("account");
        Context.Add(account);

        var transactionDetails = new TransactionDetails(100.0m, TransactionType.Income);
        var transaction = Transaction.Create(account, "Name", transactionDetails, DateTime.Now);
        
        var tag = Tag.Create(null, "tag", TagType.ForIncome);
        transaction.AddTag(tag);
        
        Context.Add(transaction);
        
        await Context.SaveChangesAsync();
        
        var request = new UpdateTransactionCommand
        {
            Id = transaction.Id,
            AccountId = account.Id,
            Name = "Renamed",
            Amount = -100.0m,
            Date = DateTime.Now,
            Type = TransactionType.Expense,
        };
        
        // Act
        await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Context.Transactions.ToList().Should().ContainSingle(x => x.Name == "Renamed");
    }
}