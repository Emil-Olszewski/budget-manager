using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Tags;

public sealed record GetAllTagsQuery() : IRequest<List<TagResponse>>;

internal sealed class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, List<TagResponse>>
{
    private readonly IContext context;

    public GetAllTagsQueryHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task<List<TagResponse>> Handle(GetAllTagsQuery request, CancellationToken ct)
    {
        return await context.Set<Tag>()
            .AsNoTracking()
            .Where(x => x.Parent == null)
            .Include(x => x.Children)
            .Select(x => TagResponse.CreateFrom(x))
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
}