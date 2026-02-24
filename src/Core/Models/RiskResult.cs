#nullable enable
using System.Collections.Generic;

namespace FixedIncomePricingLibrary.Core.Models;

/// <summary>
/// Contains the calculated risk metrics for an instrument or portfolio.
/// </summary>
public class RiskResult
{
    /// <summary>
    /// Dollar Value of 1 Basis Point.
    /// </summary>
    public double DV01 { get; init; }

    /// <summary>
    /// Modified Duration in years.
    /// </summary>
    public double ModifiedDuration { get; init; }

    /// <summary>
    /// Convexity.
    /// </summary>
    public double Convexity { get; init; }

    /// <summary>
    /// Sensitivities to specific maturity points on the curve.
    /// </summary>
    public Dictionary<string, double>? KeyRateDurations { get; init; }

    /// <summary>
    /// Sensitivity to changes in volatility.
    /// </summary>
    public double Vega { get; init; }

    /// <summary>
    /// Sensitivity to the passage of time.
    /// </summary>
    public double Theta { get; init; }
}
