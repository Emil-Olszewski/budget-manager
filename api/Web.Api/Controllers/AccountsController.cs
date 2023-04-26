using Core.Application.Features.Accounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Controllers.Common;

namespace Web.Api.Controllers;

public class AccountsController : BaseController
{
    public AccountsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts([FromQuery] GetAllAccountsQuery request, CancellationToken ct)
    {
        return Ok(await Mediator.Send(request, ct).ConfigureAwait(false));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccount(int id, CancellationToken ct)
    {
        return Ok(await Mediator.Send(new GetAccountQuery(id), ct).ConfigureAwait(false));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand request, CancellationToken ct)
    {
        await Mediator.Send(request, ct).ConfigureAwait(false);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountCommand request, CancellationToken ct)
    {
        if (id != request.Id)
        {
            throw new ArgumentException("Provided ids are different");
        }
        
        await Mediator.Send(request, ct).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(int id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteAccountCommand(id), ct).ConfigureAwait(false);
        return Ok();
    }
}