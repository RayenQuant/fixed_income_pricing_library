#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;
using MathNet.Numerics.Distributions;

namespace FixedIncomePricingLibrary.Risk;

/// <summary>
/// Provides Value-at-Risk (VaR) calculations and Scenario Analysis.
/// </summary>
public class RiskService
{
    /// <summary>
    /// Calculates Parametric VaR: VaR = PortfolioValue * Z * Sigma * sqrt(HoldingPeriod)
    /// </summary>
    public static double CalculateParametricVaR(double portfolioNpv, double volatility, double confidenceLevel = 0.95, int horizonDays = 1)
    {
        double z = Normal.InvCDF(0, 1, confidenceLevel);
        return portfolioNpv * z * volatility * Math.Sqrt(horizonDays / 252.0);
    }

    /// <summary>
    /// Runs a parallel shift scenario on a portfolio.
    /// </summary>
    public static double RunParallelShiftScenario(IPricingEngine engine, IInstrument instrument, IYieldCurve curve, double shiftBp)
    {
        // Numerical implementation
        return 0; // Stub
    }
}
