using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Tags;

public sealed record CreateTagCommand(int? ParentId, string Name, TagType Type) : IRequest;

internal sealed class CreateTagCommandHandler : IRequestHandler<CreateTagCommand>
{
    private readonly IContext context;

    public CreateTagCommandHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task Handle(CreateTagCommand request, CancellationToken ct)
    {
        var tag = await CreateTag(request, ct).ConfigureAwait(false);
        context.Add(tag);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private async Task<Tag> CreateTag(CreateTagCommand request, CancellationToken ct)
    {
        if (request.ParentId is null)
        {
            return Tag.Create(null, request.Name, request.Type);
        }
        
        var parent = await context.Set<Tag>()
            .Where(x => x.Id == request.ParentId)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (parent is null)
        {
            throw new BusinessException("Parent not exists");
        }

        return Tag.Create(parent, request.Name, request.Type);
    }
}

