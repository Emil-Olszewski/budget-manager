using Core.Domain.Entities;

namespace Core.Application.Features.Tags;

internal sealed record TagResponse(int Id, string Name, TagType Type, List<TagResponse> Children)
{
    public static TagResponse CreateFrom(Tag tag)
    {
        return new TagResponse(tag.Id, tag.Name, tag.TagType, tag.Children.Select(CreateFrom).ToList());
    }
}
