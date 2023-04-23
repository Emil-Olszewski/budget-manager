﻿using Core.Application.Features.Accounts;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Accounts;

[TestFixture]
internal sealed class CreateAccountTests : TestBase
{
    private CreateAccountCommandHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new CreateAccountCommandHandler(Context);
    }
    
    [Test]
    [TestCase("")]
    [TestCase("12345123451234512345123451234512345123451234512345123451234512345")]
    public async Task Handle_NameIsInvalid_BusinessException(string name)
    {
        // Arrange
        var command = new CreateAccountCommand(name, Currency.Pln);

        // Act && Assert
        var action = async () => await handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }
    
    [Test]
    public async Task Handle_CurrencyIsNotDefined_BusinessException()
    {
        // Arrange
        var command = new CreateAccountCommand("Test name", Currency.NotDefined);

        // Act && Assert
        var action = async () => await handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_AccountCreated()
    {
        // Arrange
        var command = new CreateAccountCommand("Test name", Currency.Pln);

        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Context.Accounts.ToList().Should().ContainSingle(x => x.Name == "Test name");
    }
    
    [Test]
    public async Task Handle_WithInitialBalance_AccountCreated()
    {
        // Arrange
        var command = new CreateAccountCommand("Test name", Currency.Pln, 15.0m);

        // Act
        await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Context.Accounts.ToList().Should().ContainSingle(x => x.Name == "Test name");
        Context.Transactions.ToList().Should()
            .ContainSingle(x =>
                x.TransactionDetails.TransactionType == TransactionType.InitialBalance 
                && x.TransactionDetails.Amount == 15.0m);
    }
}