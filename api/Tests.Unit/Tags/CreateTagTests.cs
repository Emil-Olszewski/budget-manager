using Core.Application.Features.Tags;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests.Unit.Tags;

[TestFixture]
internal sealed class CreateTagTests : TestBase
{
    private CreateTagCommandHandler handler;

    [SetUp]
    public void Prepare()
    {
        handler = new CreateTagCommandHandler(Context);
    }

    [Test]
    [TestCase("")]
    [TestCase("123451234512345123451")]
    public async Task Handle_NameIsInvalid_BusinessException(string name)
    {
        // Arrange
        var request = new CreateTagCommand(null, name, TagType.ForOutcome);
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_ParentNotExists_BusinessException()
    {
        // Arrange
        var request = new CreateTagCommand(1, "Name", TagType.ForOutcome);
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_TagCreated()
    {
        // Arrange
        var request = new CreateTagCommand(null, "Test name", TagType.ForOutcome);

        // Act
        await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Context.Tags.ToList().Should().ContainSingle(x => x.Name == "Test name");
    }

    [Test]
    public async Task Handle_ParentWithIncomeTypeProvided_BusinessException()
    {
        // Arrange
        var tag = Tag.Create(null, "Parent", TagType.ForIncome);
        Context.Add(tag);
        
        var request = new CreateTagCommand(1, "Name", TagType.ForOutcome);
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task Handle_ParentProvidedForIncomeType_BusinessException()
    {
        // Arrange
        var tag = Tag.Create(null, "Parent", TagType.ForOutcome);
        Context.Add(tag);
        
        var request = new CreateTagCommand(1, "Name", TagType.ForIncome);
        
        // Act && Assert
        var action = async () => await handler.Handle(request, CancellationToken.None);
        await action.Should().ThrowAsync<BusinessException>();
    }
    
    [Test]
    public async Task Handle_ParentProvided_TagCreated()
    {
        // Arrange
        var tag = Tag.Create(null, "Parent", TagType.ForOutcome);
        Context.Add(tag);
        await Context.SaveChangesAsync();
        
        var request = new CreateTagCommand(1, "Child", TagType.ForOutcome);

        // Act
        await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Context.Tags.ToList().Should().ContainSingle(x => x.Name == "Child");
        Context.Tags.Include(x => x.Children)
            .First().Children.Should().ContainSingle(x => x.Name == "Child");
    }
} 