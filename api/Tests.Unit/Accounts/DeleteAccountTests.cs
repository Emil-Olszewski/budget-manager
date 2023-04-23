using Core.Application.Features.Accounts;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Accounts;

[TestFixture]
internal sealed class DeleteAccountTests : TestBase
{
    private DeleteAccountCommandHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new DeleteAccountCommandHandler(Context);
    }

    [Test]
    public async Task Handle_AccountNotExists_BusinessException()
    {
        // Arrange
        var command = new DeleteAccountCommand(0);
        
        // Act && Assert
        var action = async () => await handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }
    
    [Test]
    public async Task Handle_AccountDeleted()
    {
        // Arrange
        var account = Account.Create("Name");
        Context.Add(account);
        await Context.SaveChangesAsync();
        
        var command = new DeleteAccountCommand(1);
        
        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Context.Accounts.Count().Should().Be(0);
    }
}