#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;
using FixedIncomePricingLibrary.Risk;

namespace FixedIncomePricingLibrary.Risk.VaR;

/// <summary>
/// Calculates Value-at-Risk using historical simulation.
/// </summary>
public class HistoricalVaRCalculator
{
    private readonly Portfolio _portfolio;
    private readonly IEnumerable<HistoricalRateEntry> _history;

    public HistoricalVaRCalculator(Portfolio portfolio, IEnumerable<HistoricalRateEntry> history)
    {
        _portfolio = portfolio;
        _history = history.OrderBy(h => h.Date).ToList();
    }

    /// <summary>
    /// Calculates VaR at a given confidence level.
    /// </summary>
    public VaRResult CalculateVaR(double confidenceLevel, IPricingEngine engine, IYieldCurve currentCurve)
    {
        var entries = _history.ToList();
        var pnlList = new List<double>();

        double baseNpv = _portfolio.CalculateTotalNpv(inst => engine.Price(inst).DirtyPrice);

        for (int i = 1; i < entries.Count; i++)
        {
            var prev = entries[i - 1];
            var curr = entries[i];

            double shock = curr.Rate1Y - prev.Rate1Y;

            var shockedCurve = BumpCurve(currentCurve, shock);
            var shockedEngine = RecreateEngine(engine, shockedCurve);

            double shockedNpv = _portfolio.CalculateTotalNpv(inst => shockedEngine.Price(inst).DirtyPrice);
            pnlList.Add(shockedNpv - baseNpv);
        }

        pnlList.Sort();

        int index = (int)Math.Floor((1 - confidenceLevel) * pnlList.Count);
        if (index < 0) index = 0;
        if (index >= pnlList.Count) index = pnlList.Count - 1;

        double varAmount = -pnlList[index];

        double expectedShortfall = -pnlList.Take(index + 1).Average();

        return new VaRResult
        {
            ConfidenceLevel = confidenceLevel,
            VaRAmount = varAmount,
            ExpectedShortfall = expectedShortfall,
            Method = "Historical Simulation"
        };
    }

    private IYieldCurve BumpCurve(IYieldCurve curve, double shock)
    {
        return MarketData.YieldCurves.YieldCurveBuilder.FlatCurve(curve.ValuationDate, 0.045 + shock);
    }

    private IPricingEngine RecreateEngine(IPricingEngine engine, IYieldCurve curve)
    {
        var type = engine.GetType();
        return (IPricingEngine)Activator.CreateInstance(type, curve)!;
    }
}
