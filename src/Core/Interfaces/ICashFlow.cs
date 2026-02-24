#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Enums;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the base contract for a single cash flow.
/// </summary>
public interface ICashFlow
{
    /// <summary>
    /// The date on which the payment occurs.
    /// </summary>
    DateTime PaymentDate { get; }

    /// <summary>
    /// The amount of the payment.
    /// </summary>
    double Amount { get; }

    /// <summary>
    /// The currency of the payment.
    /// </summary>
    Currency Currency { get; }
}
