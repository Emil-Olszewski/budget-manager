using System.Text;
using Core.Domain.Entities;

namespace Core.Application.Features.Transactions;

internal sealed class TransactionResponse
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; } 
    public DateTime Date { get; set; }
    public List<TagForTransactionResponse> Tags { get; set; }

    public static TransactionResponse MapFrom(Transaction transaction)
    {
        return new TransactionResponse
        {
            Id = transaction.Id,
            AccountId = transaction.Account.Id,
            Name = transaction.Name,
            Amount = transaction.TransactionDetails.Amount,
            Type = transaction.TransactionDetails.TransactionType,
            Date = transaction.Date,
            Tags = transaction.Tags.Select(TagForTransactionResponse.MapFrom).ToList()
        };
    }
}

internal sealed class TagForTransactionResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public static TagForTransactionResponse MapFrom(Tag tag)
    {
        var builder = new StringBuilder();
        return new TagForTransactionResponse
        {
            Id = tag.Id,
            Name = GetNameFor(tag, builder).ToString()
        };
    }

    private static StringBuilder GetNameFor(Tag tag, StringBuilder result)
    {
        if (tag.Parent is not null)
        {
            result = GetNameFor(tag.Parent, result);
        }

        if (!string.IsNullOrWhiteSpace(result.ToString()))
        {
            result.Append('/');
        }

        return result.Append(tag.Name);
    }
}