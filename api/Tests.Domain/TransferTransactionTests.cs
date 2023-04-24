using Core.Domain.Entities;
using Core.Domain.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Domain;

[TestFixture]
internal sealed class TransferTransactionTests
{
    [Test]
    [TestCase(0)]
    [TestCase(-30)]
    [TestCase(10.42197)]
    public void Create_InvalidAmount_BusinessException(decimal amount)
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name");
        
        // Act && Assert 
        var action = () => TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, amount);
        action.Should().Throw<BusinessException>();
    }
    
    [Test]
    public void Create_DifferentCurrenciesWithNoConversionRate_BusinessException()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name", Currency.Eur);
        
        // Act && Assert 
        var action = () => TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0M);
        action.Should().Throw<BusinessException>();
    }
    
    [Test]
    public void Create_UnnecessaryConversionRate_BusinessException()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name");
        
        // Act && Assert 
        var action = () => TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0M, 15.0M);
        action.Should().Throw<BusinessException>();
    }


    [Test]
    public void Create_InvalidConversionRate_BusinessException()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name", Currency.Eur);
        
        // Act && Assert 
        var action = () => TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0M);
        action.Should().Throw<BusinessException>();
    }

    [Test]
    public void Create_SameAccountProvided_BusinessException()
    {
        // Arrange
        var accountFrom = Account.Create("Name");

        // Act && Assert 
        var action = () => TransferTransaction.Create(accountFrom, accountFrom, DateTime.Now, 10.0M);
        action.Should().Throw<BusinessException>();
    }


    [Test]
    public void Create_TransferTransactionReturned()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name");
        var date = DateTime.Now;
        
        // Act
        var result = TransferTransaction.Create(accountFrom, accountTo, date, 10.0m);
        
        // Arrange
        result.Input.TransactionDetails.TransactionType.Should().Be(TransactionType.Transfer);
        result.Input.TransactionDetails.Amount.Should().Be(-10.0m);
        result.Input.Date.Should().Be(date);
        result.Output.TransactionDetails.TransactionType.Should().Be(TransactionType.Transfer);
        result.Output.TransactionDetails.Amount.Should().Be(10.0m);
        result.Output.Date.Should().Be(date);
        result.Amount.Should().Be(10.0m);
        result.CurrencyConversionRate.Should().BeNull();
    }
    
    [Test]
    public void Create_DifferentCurrencies_TransferTransactionReturned()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name", Currency.Eur);
        
        // Act
        var result = TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0m, 4.50M);
        
        // Arrange
        result.Input.TransactionDetails.Amount.Should().Be(-10.0m);
        result.Output.TransactionDetails.Amount.Should().Be(45.0m);
        result.Amount.Should().Be(10.0m);
        result.CurrencyConversionRate.Should().Be(4.50m);
    }

    [Test]
    public void SetAmount_ConversionRateExistedButNotProvided_BusinessException()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name", Currency.Eur);
        var transferTransaction = TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0m, 4.50M);

        // Act && Assert 
        var action = () => transferTransaction.SetAmount(15.0M);
        action.Should().Throw<BusinessException>();
    }
    
    [Test]
    public void SetAmount_ConversionRateProvidedButNotExisted_BusinessException()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name");
        var transferTransaction = TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0m);

        // Act && Assert 
        var action = () => transferTransaction.SetAmount(15.0M, 4.50M);
        action.Should().Throw<BusinessException>();
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(-30)]
    [TestCase(10.42197)]
    public void SetAmount_AmountInvalid_BusinessException(decimal amount)
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name");
        var transferTransaction = TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0m);

        // Act && Assert 
        var action = () => transferTransaction.SetAmount(amount);
        action.Should().Throw<BusinessException>(); 
    }

    [Test]
    public void SetAmount_AmountSet()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name");
        var transferTransaction = TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0m);
    }
    
    [Test]
    public void SetAmount_DifferentCurrencies_AmountSet()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name", Currency.Eur);
        var transferTransaction = TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0m, 4.50m);
        
        // Act
        transferTransaction.SetAmount(15.0m, 5.0m);
        
        // Assert
        transferTransaction.Input.TransactionDetails.Amount.Should().Be(-15.0m);
        transferTransaction.Output.TransactionDetails.Amount.Should().Be(75.0m);
        transferTransaction.Amount.Should().Be(15.0m);
        transferTransaction.CurrencyConversionRate.Should().Be(5.0m);
    }

    [Test]
    public void SetDate_DateSet()
    {
        // Arrange
        var accountFrom = Account.Create("Name");
        var accountTo = Account.Create("Name", Currency.Eur);
        var transferTransaction = TransferTransaction.Create(accountFrom, accountTo, DateTime.Now, 10.0m, 4.50m);
        var date = DateTime.Now;
        
        // Act
        transferTransaction.SetDate(date);
        
        // Assert
        transferTransaction.Input.Date.Should().Be(date);
        transferTransaction.Output.Date.Should().Be(date);
    }
}