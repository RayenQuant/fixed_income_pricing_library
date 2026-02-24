#nullable enable
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Exceptions;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.MarketData.Interpolation;

public static class InterpolatorFactory
{
    public static IInterpolator Create(InterpolationMethod method)
    {
        return method switch
        {
            InterpolationMethod.Linear => new LinearInterpolator(),
            InterpolationMethod.LogLinear => new LogLinearInterpolator(),
            InterpolationMethod.CubicSpline => new CubicSplineInterpolator(),
            InterpolationMethod.MonotoneConvex => new MonotoneConvexInterpolator(),
            _ => throw new ValidationException($"Unsupported interpolation method: {method}")
        };
    }
}
