using Core.Application.Features.Tags;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Tags;

[TestFixture]
internal sealed class UpdateTagTests : TestBase
{
    private UpdateTagCommandHandler handler;

    [SetUp]
    public void Prepare()
    {
        handler = new UpdateTagCommandHandler(Context);
    }
    
    [Test]
    [TestCase("")]
    [TestCase("123451234512345123451")]
    public async Task Handle_NameIsInvalid_BusinessException(string name)
    {
        // Arrange
        var request = new UpdateTagCommand(1, name, TagType.ForOutcome);
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_HasParentAndChangingType_BusinessException()
    {
        // Arrange
        var tag = Tag.Create(null, "Name", TagType.ForOutcome);
        var childTag = Tag.Create(tag, "Name", TagType.ForOutcome);

        Context.AddRange(tag, childTag);
        await Context.SaveChangesAsync();
        
        var request = new UpdateTagCommand(2, "Name", TagType.ForIncome);
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_HasChildrenAndChangingType_BusinessException()
    {
        // Arrange
        var tag = Tag.Create(null, "Name", TagType.ForOutcome);
        var childTag = Tag.Create(tag, "Name", TagType.ForOutcome);

        Context.AddRange(tag, childTag);
        await Context.SaveChangesAsync();
        
        var request = new UpdateTagCommand(1, "Name", TagType.ForIncome);
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_TagUdated()
    {
        // Arrange
        var tag = Tag.Create(null, "Name", TagType.ForOutcome);

        Context.Add(tag);
        await Context.SaveChangesAsync();
        
        var request = new UpdateTagCommand(1, "Renamed", TagType.ForOutcome);
        
        // Act
        await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Context.Tags.Should().ContainSingle(x => x.Name == "Renamed");
    }
}