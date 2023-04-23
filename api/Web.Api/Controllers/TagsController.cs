using Core.Application.Features.Tags;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Controllers.Common;

namespace Web.Api.Controllers;

public class TagsController : BaseController
{
    public TagsController(IMediator mediator) : base(mediator)
    {
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTags(CancellationToken ct)
    {
        return Ok(await Mediator.Send(new GetAllTagsQuery(), ct).ConfigureAwait(false));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTag(int id, CancellationToken ct)
    {
        return Ok(await Mediator.Send(new GetTagQuery(id), ct).ConfigureAwait(false));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagCommand request, CancellationToken ct)
    {
        await Mediator.Send(request, ct).ConfigureAwait(false);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagCommand request, CancellationToken ct)
    {
        if (id != request.Id)
        {
            throw new ArgumentException("Provided ids are different");
        }
        
        await Mediator.Send(request, ct).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteTagCommand(id), ct).ConfigureAwait(false);
        return Ok();
    }
}