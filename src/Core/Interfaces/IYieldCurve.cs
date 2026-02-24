#nullable enable
using System;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the contract for an interest rate yield curve.
/// </summary>
public interface IYieldCurve
{
    /// <summary>
    /// The date for which the curve is valid (valuation date).
    /// </summary>
    DateTime ValuationDate { get; }

    /// <summary>
    /// Calculates the discount factor for a given date.
    /// </summary>
    double GetDiscountFactor(DateTime date);

    /// <summary>
    /// Calculates the zero-coupon rate for a given maturity date.
    /// </summary>
    double GetZeroRate(DateTime maturity);

    /// <summary>
    /// Calculates the forward rate between two dates.
    /// </summary>
    double GetForwardRate(DateTime start, DateTime end);

    /// <summary>
    /// Calculates the par swap rate for a given tenor.
    /// </summary>
    double GetParSwapRate(string tenor);
}
