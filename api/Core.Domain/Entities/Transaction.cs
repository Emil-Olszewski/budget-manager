using Core.Domain.Common;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public enum TransactionType
{
    NotDefined = 0,
    InitialBalance = 1,
    Income = 2,
    Expense = 3,
    Transfer = 4,
}

public sealed record TransactionDetails
{
    public decimal Amount { get; init; }
    public TransactionType TransactionType { get; init; }

    public TransactionDetails(decimal amount, TransactionType transactionType)
    {
        if (transactionType == TransactionType.NotDefined)
        {
            throw new BusinessException("Transaction type must be defined");
        }
        
        switch (amount)
        {
            case 0 when transactionType != TransactionType.InitialBalance:
                throw new BusinessException("Transaction amount cannot be equal to 0");
            case > 0 when transactionType == TransactionType.Expense:
                throw new BusinessException("Expense cannot have positive amount");
            case < 0 when transactionType == TransactionType.Income:
                throw new BusinessException("Income cannot have negative amount");
        }

        Amount = amount;
        TransactionType = transactionType;
    }
}

public sealed class Transaction : AuditableBaseEntity
{
    public Account Account { get; set; }

    private string name;
    public string Name
    {
        get => name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessException("Transaction name must be provided");
            }

            if (value.Length > 50)
            {
                throw new BusinessException("Transaction name is too long");
            }
            
            name = value;
        }
    }

    private TransactionDetails transactionDetails;

    public TransactionDetails TransactionDetails
    {
        get => transactionDetails;
        set
        {
            if (transactionDetails.TransactionType != value.TransactionType)
            {
                if (tags.Any())
                {
                    throw new BusinessException("Transaction type cannot be changed when any tag is attached");
                }
                
                switch (value.TransactionType)
                {
                    case TransactionType.InitialBalance:
                    case TransactionType.Transfer:
                        throw new BusinessException("This transaction type cannot be set manually");
                }
            }

            transactionDetails = value;
        }
    }

    public DateTime Date { get; set; }

    private readonly List<Tag> tags;
    public ICollection<Tag> Tags => tags.AsReadOnly();

    private Transaction()
    {
        tags = new List<Tag>();
    }

    public static Transaction Create(Account account, string name, TransactionDetails transactionDetails, DateTime dateTime)
    {
        return new Transaction
        {
            Account = account,
            Name = name,
            transactionDetails = transactionDetails,
            Date = dateTime
        };
    }

    public void AddTag(Tag tag)
    {
        switch (tag.TagType)
        {
            case TagType.NotDefined: 
            case TagType.ForIncome when TransactionDetails.TransactionType == TransactionType.Expense:
            case TagType.ForOutcome when TransactionDetails.TransactionType == TransactionType.Income:
                throw new BusinessException("Invalid tag type");
        }

        if (tags.Count >= 5)
        {
            throw new BusinessException("Cannot add more tags");
        }
        
        tags.Add(tag);
    }

    public void RemoveTag(Tag tag)
    {
        if (!tags.Contains(tag))
        {
            throw new BusinessException("Tag does not belong to this transaction");
        }

        tags.Remove(tag);
    }
}