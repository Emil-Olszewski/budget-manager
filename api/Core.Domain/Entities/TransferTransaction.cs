using Core.Domain.Common;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class TransferTransaction : BaseEntity
{
    private readonly Transaction input;
    /// <summary>
    /// Describes where the amount comes from
    /// </summary>
    public Transaction Input
    {
        get => input;
        init
        {
            if (value.TransactionDetails.TransactionType != TransactionType.Transfer)
            {
                throw new BusinessException("Input transaction must be of the transfer type");
            }
            
            input = value;
        }
    }
    
    private readonly Transaction output;
    /// <summary>
    /// Describes where the amount goes to
    /// </summary>
    public Transaction Output
    {
        get => output;
        init
        {
            if (value.TransactionDetails.TransactionType != TransactionType.Transfer)
            {
                throw new BusinessException("Output transaction must be of the transfer type");
            }
            
            output = value;
        }
    }

    private decimal amount;
    public decimal Amount
    {
        get => amount;
        set
        {
            if (0 >= value)
            {
                throw new BusinessException("Transaction transfer amount cannot be lower than 0");
            }
            
            if (value % 0.0001m != 0)
            {
                throw new BusinessException("Amount has too high precision");
            }

            amount = value;
        }
    }

    private decimal? currencyConversionRate;

    public decimal? CurrencyConversionRate
    {
        get => currencyConversionRate;
        set
        {
            if (0 >= value)
            {
                throw new BusinessException("Currency conversion rate cannot be equal or lower than 0");
            }
            
            currencyConversionRate = value;
        }
    }

    private TransferTransaction()
    {
        
    }

    public static TransferTransaction Create(Account from, Account to, DateTime date, decimal amount,
        decimal? currencyConversionRate = null)
    {
        if (from == to)
        {
            throw new BusinessException("Cannot transfer within one account");
        }
        
        if (from.Currency != to.Currency && currencyConversionRate is null)
        {
            throw new BusinessException(
                "Currency conversion rate must be provided when transferring amounts between accounts with different currencies");
        }

        if (from.Currency == to.Currency && currencyConversionRate is not null)
        {
            throw new BusinessException(
                "Accounts are of the same currency, so no currency conversion rate should be provided");
        }

        decimal multiplier = currencyConversionRate ?? 1;
        
        var inputTransactionDetails = new TransactionDetails(-amount, TransactionType.Transfer);
        var inputTransaction = Transaction.Create(from, "Transfer transaction", inputTransactionDetails, date);

        var outputTransactionDetails = new TransactionDetails(amount * multiplier, TransactionType.Transfer);
        var outputTransaction = Transaction.Create(to, "Transfer transaction", outputTransactionDetails, date);

        return new TransferTransaction
        {
            Input = inputTransaction,
            Output = outputTransaction,
            Amount = amount,
            CurrencyConversionRate = currencyConversionRate,
        };
    }

    public void SetAmount(decimal newAmount, decimal? newConversionRate = null)
    {
        if (CurrencyConversionRate is null != newConversionRate is null)
        {
            throw new BusinessException("Cannot add or remove conversion rate to existing transfer");
        }

        Amount = newAmount;
        CurrencyConversionRate = newConversionRate;
        
        input.TransactionDetails = new TransactionDetails(-newAmount, TransactionType.Transfer);
                
        decimal multiplier = newConversionRate ?? 1;
        output.TransactionDetails = new TransactionDetails(newAmount * multiplier, TransactionType.Transfer);
    }

    public void SetDate(DateTime date)
    {
        input.Date = output.Date = date;
    }
}