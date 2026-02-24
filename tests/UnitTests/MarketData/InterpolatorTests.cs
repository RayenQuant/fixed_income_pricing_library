using System;
using FixedIncomePricingLibrary.MarketData.Interpolation;
using Xunit;

namespace FixedIncomePricingLibrary.UnitTests.MarketData;

public class InterpolatorTests
{
    [Fact]
    public void LinearInterpolator_InterpolatesCorrectly()
    {
        var xs = new[] { 0.0, 1.0, 2.0 };
        var ys = new[] { 0.0, 10.0, 20.0 };
        var interpolator = new LinearInterpolator();
        interpolator.Initialize(xs, ys);

        Assert.Equal(5.0, interpolator.Interpolate(0.5));
        Assert.Equal(15.0, interpolator.Interpolate(1.5));
    }

    [Fact]
    public void LogLinearInterpolator_InterpolatesCorrectly()
    {
        var xs = new[] { 0.0, 1.0 };
        var ys = new[] { 1.0, Math.Exp(1) };
        var interpolator = new LogLinearInterpolator();
        interpolator.Initialize(xs, ys);

        // ln(y) = 0 + (1-0)*(0.5-0)/(1-0) = 0.5
        // y = exp(0.5)
        Assert.Equal(Math.Exp(0.5), interpolator.Interpolate(0.5), 10);
    }
}
