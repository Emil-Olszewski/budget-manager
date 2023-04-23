using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Tags;

public sealed record DeleteTagCommand(int Id) : IRequest;

internal sealed class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
{
    private readonly IContext context;

    public DeleteTagCommandHandler(IContext context)
    {
        this.context = context;
    }
    
    public async Task Handle(DeleteTagCommand request, CancellationToken ct)
    {
        var tag = await context.Set<Tag>()
            .Include(x => x.Children)
            .Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(ct)
            .ConfigureAwait(false);

        if (tag is null)
        {
            throw new BusinessException("Account was not found");
        }
        
        DeleteTagWithChildren(tag);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private void DeleteTagWithChildren(Tag tag)
    {
        foreach (var child in tag.Children)
        {
            DeleteTagWithChildren(child);
        }
        
        context.Remove(tag);
    }
}
