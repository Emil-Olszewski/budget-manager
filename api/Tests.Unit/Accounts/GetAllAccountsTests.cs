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
}