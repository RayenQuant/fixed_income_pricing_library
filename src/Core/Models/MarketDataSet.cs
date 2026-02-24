#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.Core.Models;

/// <summary>
/// Container for a consistent set of market data.
/// </summary>
public class MarketDataSet
{
    /// <summary>
    /// The valuation date for this data set.
    /// </summary>
    public DateTime ValuationDate { get; init; }

    /// <summary>
    /// Yield curves indexed by currency.
    /// </summary>
    public Dictionary<Currency, IYieldCurve> YieldCurves { get; init; } = new();

    /// <summary>
    /// Volatility surfaces indexed by type (e.g., "Swaption", "CapFloor").
    /// </summary>
    public Dictionary<string, IVolatilitySurface> VolatilitySurfaces { get; init; } = new();

    /// <summary>
    /// Rate fixings indexed by index type.
    /// </summary>
    public Dictionary<FloatingRateIndex, double> Fixings { get; init; } = new();
}
