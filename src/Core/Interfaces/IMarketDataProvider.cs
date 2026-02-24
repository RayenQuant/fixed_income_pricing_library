#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Enums;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the contract for a provider of curves, surfaces, and market fixings.
/// </summary>
public interface IMarketDataProvider
{
    /// <summary>
    /// Retrieves a yield curve for the specified currency and date.
    /// </summary>
    IYieldCurve GetYieldCurve(Currency currency, DateTime date);

    /// <summary>
    /// Retrieves a volatility surface for the specified type and date.
    /// </summary>
    IVolatilitySurface GetVolSurface(string type, DateTime date);

    /// <summary>
    /// Retrieves a benchmark rate fixing for the specified index and date.
    /// </summary>
    double GetFixingRate(FloatingRateIndex index, DateTime date);
}
