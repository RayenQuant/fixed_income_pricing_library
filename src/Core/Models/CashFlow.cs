#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Core.Models;

/// <summary>
/// Implementation of a single cash flow.
/// </summary>
public record CashFlow : ICashFlow
{
    public DateTime PaymentDate { get; init; }
    public double Amount { get; init; }
    public Currency Currency { get; init; }
    public double Notional { get; init; }
    public double DayCountFraction { get; init; }

    public CashFlow(DateTime paymentDate, double amount, Currency currency)
    {
        PaymentDate = paymentDate;
        Amount = amount;
        Currency = currency;
    }
}
