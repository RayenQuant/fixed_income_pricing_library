#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.MarketData.Volatility;

/// <summary>
/// Implements a 3D volatility cube for swaptions (Expiry x Tenor x Strike).
/// </summary>
public class SwaptionVolatilityCube : IVolatilitySurface
{
    // Simplified implementation using a flat vol for now, 
    // but structure is ready for 3D interpolation.
    private readonly double _flatVol;

    public SwaptionVolatilityCube(double flatVol)
    {
        _flatVol = flatVol;
    }

    public double GetVolatility(DateTime expiry, string tenor, double strike)
    {
        return _flatVol;
    }
}
