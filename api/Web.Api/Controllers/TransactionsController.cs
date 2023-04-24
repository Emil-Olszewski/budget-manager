using Core.Application.Features.Transactions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Controllers.Common;

namespace Web.Api.Controllers;

public class TransactionsController : BaseController
{
    public TransactionsController(IMediator mediator) : base(mediator)
    {
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllTransactions(CancellationToken ct)
    {
        return Ok(await Mediator.Send(new GetAllTransactionsQuery(), ct).ConfigureAwait(false));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(int id, CancellationToken ct)
    {
        return Ok(await Mediator.Send(new GetTransactionQuery(id), ct).ConfigureAwait(false));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand request, CancellationToken ct)
    {
        await Mediator.Send(request, ct).ConfigureAwait(false);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, [FromBody] UpdateTransactionCommand request, CancellationToken ct)
    {
        if (id != request.Id)
        {
            throw new ArgumentException("Provided ids are different");
        }
        
        await Mediator.Send(request, ct).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteTransactionCommand(id), ct).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet("TransferTransactions")]
    public async Task<IActionResult> GetAllTransferTransactions(CancellationToken ct)
    {
        return Ok(await Mediator.Send(new GetAllTransferTransactionsQuery(), ct).ConfigureAwait(false));
    }

    [HttpGet("TransferTransactions/{id}")]
    public async Task<IActionResult> GetTransferTransaction(int id, CancellationToken ct)
    {
        return Ok(await Mediator.Send(new GetTransferTransactionQuery(id), ct).ConfigureAwait(false));
    }

    [HttpPost("TransferTransactions")]
    public async Task<IActionResult> CreateTransferTransaction([FromBody] CreateTransferTransactionCommand request, CancellationToken ct)
    {
        await Mediator.Send(request, ct).ConfigureAwait(false);
        return Ok();
    }

    [HttpPut("TransferTransactions/{id}")]
    public async Task<IActionResult> UpdateTransferTransaction(int id, [FromBody] UpdateTransferTransactionCommand request, CancellationToken ct)
    {
        if (id != request.Id)
        {
            throw new ArgumentException("Provided ids are different");
        }

        await Mediator.Send(request, ct).ConfigureAwait(false);
        return Ok();
    }
}