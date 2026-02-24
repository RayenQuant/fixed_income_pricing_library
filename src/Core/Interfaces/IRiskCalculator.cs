#nullable enable
using FixedIncomePricingLibrary.Core.Models;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the contract for calculating risk metrics (Greeks).
/// </summary>
public interface IRiskCalculator
{
    /// <summary>
    /// Calculates greeks for an instrument.
    /// </summary>
    RiskResult CalculateRisk(IInstrument instrument, IYieldCurve curve, IVolatilitySurface? volSurface = null);

    /// <summary>
    /// Calculates aggregates risk for a portfolio.
    /// </summary>
    RiskResult CalculatePortfolioRisk(System.Collections.Generic.IEnumerable<IInstrument> portfolio, IYieldCurve curve, IVolatilitySurface? volSurface = null);
}
