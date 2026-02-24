#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Core.Models;

/// <summary>
/// Contains the resulting values from a pricing operation.
/// </summary>
public class PricingResult
{
    /// <summary>
    /// The instrument that was priced.
    /// </summary>
    public IInstrument Instrument { get; }

    /// <summary>
    /// The clean price of the instrument (excluding accrued interest).
    /// </summary>
    public double CleanPrice { get; init; }

    /// <summary>
    /// The dirty price of the instrument (including accrued interest).
    /// </summary>
    public double DirtyPrice { get; init; }

    /// <summary>
    /// The accrued interest at the settlement date.
    /// </summary>
    public double AccruedInterest { get; init; }

    /// <summary>
    /// The yield to maturity (for bonds) if calculated.
    /// </summary>
    public double? YieldToMaturity { get; init; }

    /// <summary>
    /// The date and time when the pricing was performed.
    /// </summary>
    public DateTime Timestamp { get; init; }

    public PricingResult(IInstrument instrument)
    {
        Instrument = instrument;
        Timestamp = DateTime.UtcNow;
    }
}
