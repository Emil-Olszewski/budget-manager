using Core.Application.Features.Tags;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Tags;

[TestFixture]
internal sealed class DeleteTagTests : TestBase
{
    private DeleteTagCommandHandler handler;

    [SetUp]
    public void Prepare()
    {
        handler = new DeleteTagCommandHandler(Context);
    }

    [Test]
    public async Task Handle_TagNotExists_BusinessException()
    {
        // Arrange
        var request = new DeleteTagCommand(1);

        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_TagDeleted()
    {
        // Arrange
        var tag = Tag.Create(null, "Name", TagType.ForIncome);
        Context.Add(tag);
        await Context.SaveChangesAsync();

        var request = new DeleteTagCommand(1);
        
        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        Context.Tags.Count().Should().Be(0);
    }
    
    [Test]
    public async Task Handle_WithChildren_TagDeleted()
    {
        // Arrange
        var parent = Tag.Create(null, "Name", TagType.ForOutcome);
        var child = Tag.Create(parent, "Name", TagType.ForOutcome);
        Context.AddRange(parent, child);
        await Context.SaveChangesAsync();

        var request = new DeleteTagCommand(1);
        
        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        Context.Tags.Count().Should().Be(0);
    }
}