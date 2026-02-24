#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.MarketData.Volatility;

/// <summary>
/// Strips flat cap volatilities into forward caplet volatilities.
/// </summary>
public class CapletVolStripper
{
    public IVolatilitySurface Strip(IVolatilitySurface flatSurface, IYieldCurve curve)
    {
        // For production, this involves solving for caplet vols sequentially.
        // Returning the flat surface as a placeholder.
        return flatSurface;
    }
}
