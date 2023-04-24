using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Transactions;

internal sealed class TransferTransactionShortResponse
{
    public int Id { get; set; }
    public int AccountFromId { get; set; }
    public int AccountToId { get; set; }
    public int InputTransactionId { get; set; }
    public int OutputTransactionId { get; set; }
}

public sealed record GetAllTransferTransactionsQuery : IRequest<List<TransferTransactionShortResponse>>;

internal sealed class GetAllTransferTransactionsQueryHandler : IRequestHandler<GetAllTransferTransactionsQuery, List<TransferTransactionShortResponse>>
{
    private readonly IContext context;

    public GetAllTransferTransactionsQueryHandler(IContext context)
    {
        this.context = context;
    }

    public async Task<List<TransferTransactionShortResponse>> Handle(GetAllTransferTransactionsQuery request, CancellationToken ct)
    {
        return await context.Set<TransferTransaction>()
           .AsNoTracking()
           .Select(x => new TransferTransactionShortResponse
           {
               Id = x.Id,
               AccountFromId = x.Input.Account.Id,
               AccountToId = x.Output.Account.Id,
               InputTransactionId = x.Input.Id,
               OutputTransactionId = x.Output.Id,
           })
           .ToListAsync(ct)
           .ConfigureAwait(false);
    }
}

