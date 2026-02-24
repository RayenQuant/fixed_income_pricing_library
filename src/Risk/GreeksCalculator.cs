using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;
using FixedIncomePricingLibrary.MarketData.Interpolation;
using FixedIncomePricingLibrary.MarketData.YieldCurves;

namespace FixedIncomePricingLibrary.Risk;

/// <summary>
/// Calculates sensitivities (Greeks) for fixed income instruments.
/// Uses a Func-based engine factory to correctly reconstruct any engine type
/// (including BlackEngine which requires vol surfaces, not just a curve).
/// </summary>
public static class GreeksCalculator
{
    private const double Bp = 0.0001;

    /// <summary>
    /// Calculates DV01 (Dollar Value of 01) using a user-supplied engine factory.
    /// DV01 = -(V(r + 1bp) - V(r))
    /// </summary>
    public static double CalculateDV01(Func<IYieldCurve, IPricingEngine> engineFactory, IInstrument instrument, IYieldCurve curve)
    {
        var engine = engineFactory(curve);
        double v0 = engine.Price(instrument).DirtyPrice;

        var bumpedCurve = BumpCurve(curve, Bp);
        var bumpedEngine = engineFactory(bumpedCurve);
        double vUp = bumpedEngine.Price(instrument).DirtyPrice;

        return -(vUp - v0);
    }

    /// <summary>
    /// Calculates Modified Duration using a user-supplied engine factory.
    /// D_mod = -[V(+dr) - V(-dr)] / (2 × V × dr)
    /// </summary>
    public static double CalculateModifiedDuration(Func<IYieldCurve, IPricingEngine> engineFactory, IInstrument instrument, IYieldCurve curve)
    {
        var engine = engineFactory(curve);
        double v0 = engine.Price(instrument).DirtyPrice;

        var curveUp = BumpCurve(curve, Bp);
        double vUp = engineFactory(curveUp).Price(instrument).DirtyPrice;

        var curveDown = BumpCurve(curve, -Bp);
        double vDown = engineFactory(curveDown).Price(instrument).DirtyPrice;

        return -(vUp - vDown) / (2 * v0 * Bp);
    }

    /// <summary>
    /// Legacy overload: tries to recreate the engine via reflection.
    /// Only works for engines with a single IYieldCurve constructor parameter.
    /// </summary>
    public static double CalculateDV01(IPricingEngine engine, IInstrument instrument, IYieldCurve curve)
    {
        double v0 = engine.Price(instrument).DirtyPrice;
        var bumpedCurve = BumpCurve(curve, Bp);

        var engineType = engine.GetType();
        var bumpedEngine = (IPricingEngine)Activator.CreateInstance(engineType, bumpedCurve)!;
        double vUp = bumpedEngine.Price(instrument).DirtyPrice;

        return -(vUp - v0);
    }

    /// <summary>
    /// Legacy overload for Modified Duration.
    /// </summary>
    public static double CalculateModifiedDuration(IPricingEngine engine, IInstrument instrument, IYieldCurve curve)
    {
        double v0 = engine.Price(instrument).DirtyPrice;
        var engineType = engine.GetType();

        var curveUp = BumpCurve(curve, Bp);
        var engineUp = (IPricingEngine)Activator.CreateInstance(engineType, curveUp)!;
        double vUp = engineUp.Price(instrument).DirtyPrice;

        var curveDown = BumpCurve(curve, -Bp);
        var engineDown = (IPricingEngine)Activator.CreateInstance(engineType, curveDown)!;
        double vDown = engineDown.Price(instrument).DirtyPrice;

        return -(vUp - vDown) / (2 * v0 * Bp);
    }

    private static IYieldCurve BumpCurve(IYieldCurve curve, double bump)
    {
        double currentZero = curve.GetZeroRate(curve.ValuationDate.AddYears(1));
        double bumpedZero = currentZero + bump;

        var times = new[] { 0.0, 50.0 };
        var dfs = new[] { 1.0, Math.Exp(-bumpedZero * 50.0) };
        return new YieldCurve(curve.ValuationDate, times, dfs, new LogLinearInterpolator());
    }
}
