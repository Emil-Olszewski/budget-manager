using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Features.Transactions;
using Core.Domain.Entities;
using FluentAssertions;

namespace Tests.Unit.Transactions;

[TestFixture]
internal sealed class GetAllTransferTransactionsTests : TestBase
{
    private GetAllTransferTransactionsQueryHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new GetAllTransferTransactionsQueryHandler(Context);     
    }
    
    [Test]
    public async Task Handle_TransfersReturned()
    {
        Assert.Fail();
    }
    
    [Test]
    public async Task Handle_DatesProvided_TransfersReturned()
    {
        // Arrange
        var accountFrom = Account.Create("AccountFrom");
        var accountTo = Account.Create("AccountTo");
        var transaction1 = TransferTransaction.Create(accountFrom, accountTo, new DateTime(2023, 01, 14), 10.0m);
        var transaction2 = TransferTransaction.Create(accountFrom, accountTo, new DateTime(2023, 01, 15), 10.0m);
        var transaction3 = TransferTransaction.Create(accountFrom, accountTo, new DateTime(2023, 02, 15), 10.0m);
        var transaction4 = TransferTransaction.Create(accountFrom, accountTo, new DateTime(2023, 02, 16), 10.0m);

        Context.AddRange(transaction1, transaction2, transaction3, transaction4);
        await Context.SaveChangesAsync();

        var request = new GetAllTransferTransactionsQuery
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
