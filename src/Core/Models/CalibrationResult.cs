#nullable enable
using System.Collections.Generic;

namespace FixedIncomePricingLibrary.Core.Models;

/// <summary>
/// Contains the outcome of a model calibration.
/// </summary>
public class CalibrationResult
{
    /// <summary>
    /// The resulting calibrated parameters.
    /// </summary>
    public Dictionary<string, double> Parameters { get; init; } = new();

    /// <summary>
    /// Root Mean Square Error of the fit to market data.
    /// </summary>
    public double RMSE { get; init; }

    /// <summary>
    /// Number of iterations performed by the optimizer.
    /// </summary>
    public int Iterations { get; init; }

    /// <summary>
    /// Indicates whether the optimizer converged successfully.
    /// </summary>
    public bool Converged { get; init; }

    /// <summary>
    /// Per-instrument errors for validation.
    /// </summary>
    public Dictionary<string, double>? PerInstrumentErrors { get; init; }
}
