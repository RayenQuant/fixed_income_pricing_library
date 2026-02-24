#nullable enable
using System;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.MarketData.Interpolation;

/// <summary>
/// Implements log-linear interpolation. 
/// Interpolates linearly between the natural logarithms of the values.
/// Common for discount factors to ensure they follow an exponential decay.
/// </summary>
public class LogLinearInterpolator : IInterpolator
{
    private double[] _xs = Array.Empty<double>();
    private double[] _ys = Array.Empty<double>();

    public void Initialize(double[] xs, double[] ys)
    {
        if (xs.Length != ys.Length || xs.Length < 2)
            throw new ArgumentException("Invalid data points for interpolation.");

        _xs = xs;
        _ys = ys;
    }

    public double Interpolate(double x)
    {
        if (x <= _xs[0]) return _ys[0];
        if (x >= _xs[^1]) return _ys[^1];

        int i = Array.BinarySearch(_xs, x);
        if (i < 0) i = ~i - 1;

        double x0 = _xs[i];
        double x1 = _xs[i + 1];
        double y0 = _ys[i];
        double y1 = _ys[i + 1];

        // Interpolate linearly on ln(y)
        double lnY0 = Math.Log(y0);
        double lnY1 = Math.Log(y1);
        double lnY = lnY0 + (lnY1 - lnY0) * (x - x0) / (x1 - x0);

        return Math.Exp(lnY);
    }
}
