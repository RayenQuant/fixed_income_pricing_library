#nullable enable

namespace FixedIncomePricingLibrary.Core.Enums;

/// <summary>
/// Specifies the interpolation method for curves.
/// </summary>
public enum InterpolationMethod
{
    /// <summary>
    /// Linear interpolation on the values.
    /// </summary>
    Linear,

    /// <summary>
    /// Linear interpolation on the natural logarithm of the values (common for discount factors).
    /// </summary>
    LogLinear,

    /// <summary>
    /// Cubic spline interpolation for a smooth curve.
    /// </summary>
    CubicSpline,

    /// <summary>
    /// Monotone convex interpolation to ensure non-negative forward rates.
    /// </summary>
    MonotoneConvex
}
