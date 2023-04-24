using Core.Application.Features.Transactions;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Transactions;

internal sealed class GetTransferTransactionTests : TestBase
{
    private GetTransferTransactionQueryHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new GetTransferTransactionQueryHandler(Context);
    }

    [Test]
    public async Task Handle_TransferNotExists_BusinessException()
    {
        var request = new GetTransferTransactionQuery(1);
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>(); 
    }

    [Test]
    public async Task Handle_TransferReturned()
    {
        // Arrange
        var accountFrom = Account.Create("AccountFrom");
        var accountTo = Account.Create("AccountTo");
        var transaction = TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0m);

        Context.Add(transaction);
        await Context.SaveChangesAsync();

        var request = new GetTransferTransactionQuery(1);
        
        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        result.AccountFrom.Id.Should().Be(accountFrom.Id);
        result.AccountFrom.Name.Should().Be(accountFrom.Name);
        result.AccountTo.Id.Should().Be(accountTo.Id);
        result.AccountTo.Name.Should().Be(accountTo.Name);
        result.Date.Should().Be(transaction.Input.Date);
        result.InputAmount.Should().Be(-10.0m);
        result.OutputAmount.Should().Be(10.0m);
        result.CurrencyConversionRate.Should().BeNull();
    }
    
    [Test]
    public async Task Handle_WithCurrencyConversion_TransferReturned()
    {
        // Arrange
        var accountFrom = Account.Create("AccountFrom");
        var accountTo = Account.Create("AccountTo", Currency.Eur);
        var transaction = TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0m, 2.0m);

        Context.Add(transaction);
        await Context.SaveChangesAsync();

        var request = new GetTransferTransactionQuery(1);
        
        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        result.InputAmount.Should().Be(-10.0m);
        result.OutputAmount.Should().Be(20.0m);
        result.CurrencyConversionRate.Should().Be(2.0m);
    }
}