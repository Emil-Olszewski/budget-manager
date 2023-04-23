using Core.Application.Features.Tags;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Tags;

[TestFixture]
internal sealed class GetTagTests : TestBase
{
    private GetTagQueryHandler handler;
    
    [SetUp]
    public void Prepare()
    {
        handler = new GetTagQueryHandler(Context);
    }

    [Test]
    public async Task Handle_TagNotExists_BusinessException()
    {
        // Arrange
        var request = new GetTagQuery(1);
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_TagReturned()
    {
        // Arrange
        Context.Add(Tag.Create(null, "Name", TagType.ForOutcome));
        await Context.SaveChangesAsync();

        var request = new GetTagQuery(1);
        
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        
        // Arrange
        result.Name.Should().Be("Name");
    }
}