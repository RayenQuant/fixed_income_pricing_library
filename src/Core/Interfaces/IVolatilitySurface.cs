#nullable enable
using System;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the contract for an interest rate volatility surface or cube.
/// </summary>
public interface IVolatilitySurface
{
    /// <summary>
    /// Retrieves the volatility for a given option expiry, underlying tenor, and strike.
    /// </summary>
    double GetVolatility(DateTime expiry, string tenor, double strike);
}
