using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Tags;

public sealed record UpdateTagCommand(int Id, string Name, TagType Type) : IRequest;

internal sealed record UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand>
{
    private readonly IContext context;

    public UpdateTagCommandHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task Handle(UpdateTagCommand request, CancellationToken ct)
    {
        var tag = await context.Set<Tag>()
            .Where(x => x.Id == request.Id)
            .Include(x => x.Parent)
            .Include(x => x.Children)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (tag is null)
        {
            throw new BusinessException("Tag was not found");
        }

        if (tag.TagType != request.Type)
        {
            if (tag.Parent is not null)
            {
                throw new BusinessException("Cannot change type of tag with parent");
            }
            
            if (tag.Children.Any())
            {
                throw new BusinessException("Cannot change type of tag with children");
            }
        }

        tag.Name = request.Name;
        tag.TagType = request.Type;
        
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
