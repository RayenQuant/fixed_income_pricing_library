#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Interfaces;
using MathNet.Numerics.Interpolation;

namespace FixedIncomePricingLibrary.MarketData.Interpolation;

/// <summary>
/// Implements natural cubic spline interpolation using MathNet.Numerics.
/// </summary>
public class CubicSplineInterpolator : IInterpolator
{
    private IInterpolation? _spline;

    public void Initialize(double[] xs, double[] ys)
    {
        _spline = CubicSpline.InterpolateNaturalSorted(xs, ys);
    }

    public double Interpolate(double x)
    {
        if (_spline == null) throw new InvalidOperationException("Interpolator not initialized.");
        return _spline.Interpolate(x);
    }
}
