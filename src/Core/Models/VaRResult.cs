#nullable enable

namespace FixedIncomePricingLibrary.Core.Models;

/// <summary>
/// Contains Value at Risk (VaR) calculation results.
/// </summary>
public class VaRResult
{
    /// <summary>
    /// The confidence level (e.g., 0.95 or 0.99).
    /// </summary>
    public double ConfidenceLevel { get; init; }

    /// <summary>
    /// The calculated Value at Risk amount (expressed as a positive loss).
    /// </summary>
    public double VaRAmount { get; init; }

    /// <summary>
    /// The Expected Shortfall (Conditional VaR).
    /// </summary>
    public double ExpectedShortfall { get; init; }

    /// <summary>
    /// The method used (Historical or Monte Carlo).
    /// </summary>
    public string Method { get; init; } = string.Empty;
}
