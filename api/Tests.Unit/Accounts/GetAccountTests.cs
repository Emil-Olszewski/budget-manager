using Core.Application.Features.Accounts;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Accounts;

[TestFixture]
internal sealed class GetAccountTests : TestBase
{
    private GetAccountQueryHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new GetAccountQueryHandler(Context);
    }

    [Test]
    public async Task Handle_AccountNotExists_BusinessException()
    {
        // Arrange
        var request = new GetAccountQuery(1);
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_AccountReturned()
    {
        // Arrange
        var accounts = new List<Account>
        {
            Account.Create("Account1"),
        };

        Context.AddRange(accounts);
        await Context.SaveChangesAsync();

        var request = new GetAccountQuery(1);
        
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        
        // Arrange
        result.Name.Should().Be("Account1");
    }
}
