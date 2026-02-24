#nullable enable
using System;
using System.Linq;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.MarketData.Interpolation;

/// <summary>
/// Implements standard linear interpolation between data points.
/// </summary>
public class LinearInterpolator : IInterpolator
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

        return y0 + (y1 - y0) * (x - x0) / (x1 - x0);
    }
}
