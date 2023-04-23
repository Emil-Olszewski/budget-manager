using Core.Application.Features.Tags;
using Core.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Unit.Tags;

[TestFixture]
internal sealed class GetAllTagsTests : TestBase
{
    private GetAllTagsQueryHandler handler;

    [SetUp]
    public void Prepare()
    {
        handler = new GetAllTagsQueryHandler(Context);
    }

    [Test]
    public async Task Handle_TagsReturned()
    {
        // Arrange
        var tag1 = Tag.Create(null, "Tag1", TagType.ForOutcome);
        var tag2 = Tag.Create(null, "Tag2", TagType.ForOutcome);
        var tag12 = Tag.Create(tag1, "Tag1-2", TagType.ForOutcome);
        
        Context.AddRange(tag1, tag2, tag12);
        await Context.SaveChangesAsync();

        var request = new GetAllTagsQuery();
        
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        
        // Assert
        result.Should().HaveCount(2);
        result.First().Children.Should().HaveCount(1);
    }
}