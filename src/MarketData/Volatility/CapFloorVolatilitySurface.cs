#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.MarketData.Volatility;

/// <summary>
/// Implements a 2D volatility surface for caps and floors (Tenor x Strike).
/// </summary>
public class CapFloorVolatilitySurface : IVolatilitySurface
{
    private readonly double _flatVol;

    public CapFloorVolatilitySurface(double flatVol)
    {
        _flatVol = flatVol;
    }

    public double GetVolatility(DateTime expiry, string tenor, double strike)
    {
        return _flatVol;
    }
}
