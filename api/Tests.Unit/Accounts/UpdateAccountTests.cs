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
        var command = new UpdateAccountCommand(1, "Name");
        
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
        
        var command = new UpdateAccountCommand(1, name);

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
        
        var command = new UpdateAccountCommand(1, "Test name");

        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Context.Accounts.ToList().Should().ContainSingle(x => x.Name == "Test name");
    }
}