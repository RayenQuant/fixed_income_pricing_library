#nullable enable
using System.Collections.Generic;

namespace FixedIncomePricingLibrary.Core.Interfaces;

/// <summary>
/// Defines the contract for mathematical interpolation between data points.
/// </summary>
public interface IInterpolator
{
    /// <summary>
    /// Initializes the interpolator with a set of data points.
    /// </summary>
    void Initialize(double[] xs, double[] ys);

    /// <summary>
    /// Calculates the interpolated value for a given x.
    /// </summary>
    double Interpolate(double x);
}
