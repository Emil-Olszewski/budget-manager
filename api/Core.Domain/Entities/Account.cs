using Core.Domain.Common;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public enum Currency
{
    NotDefined = 0,
    Pln = 1,
    Eur = 2,
    Usd = 3,
}

public sealed class Account : AuditableBaseEntity
{
    private string name;
    public string Name
    {
        get => name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessException("Account name must be provided");
            }

            if (value.Length > 30)
            {
                throw new BusinessException("Account name is too long");
            }

            name = value;
        }
    }

    private Currency currency;

    public Currency Currency
    {
        get => currency;
        set
        {
            if (value == Currency.NotDefined)
            {
                throw new BusinessException("Currency must be defined");
            }

            currency = value;
        }
    }

    private readonly List<Transaction> transactions;
    public ICollection<Transaction> Transactions => transactions.AsReadOnly();
    
    private Account()
    {
        transactions = new List<Transaction>();
    }

    public static Account Create(string name, Currency currency = Currency.Pln)
    {
        return new Account
        {
            Name = name,
            Currency = currency
        };
    }
}