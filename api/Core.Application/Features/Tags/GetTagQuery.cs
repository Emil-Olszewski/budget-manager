using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Tags;

public sealed record GetTagQuery(int Id) : IRequest<TagResponse>;

internal sealed class GetTagQueryHandler : IRequestHandler<GetTagQuery, TagResponse>
{
    private readonly IContext context;

    public GetTagQueryHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task<TagResponse> Handle(GetTagQuery request, CancellationToken ct)
    {
        var tag = await context.Set<Tag>()
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => TagResponse.CreateFrom(x))
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (tag is null)
        {
            throw new BusinessException("Tag was not found");
        }

        return tag;
    }
}