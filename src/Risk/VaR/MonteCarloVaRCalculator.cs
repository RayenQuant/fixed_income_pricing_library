#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;
using FixedIncomePricingLibrary.Risk;
using MathNet.Numerics.Distributions;

namespace FixedIncomePricingLibrary.Risk.VaR;

/// <summary>
/// Calculates Value-at-Risk using Monte Carlo simulation.
/// </summary>
public class MonteCarloVaRCalculator
{
    private readonly Portfolio _portfolio;
    private readonly int _simulations;

    public MonteCarloVaRCalculator(Portfolio portfolio, int simulations = 10000)
    {
        _portfolio = portfolio;
        _simulations = simulations;
    }

    public VaRResult CalculateVaR(double confidenceLevel, IPricingEngine engine, IYieldCurve currentCurve, double vol)
    {
        var pnlList = new List<double>();
        var random = new Random();
        double baseNpv = _portfolio.CalculateTotalNpv(inst => engine.Price(inst).DirtyPrice);

        for (int i = 0; i < _simulations; i++)
        {
            double shock = Normal.Sample(random, 0, vol);
            var shockedCurve = BumpCurve(currentCurve, shock);
            var shockedEngine = RecreateEngine(engine, shockedCurve);

            double shockedNpv = _portfolio.CalculateTotalNpv(inst => shockedEngine.Price(inst).DirtyPrice);
            pnlList.Add(shockedNpv - baseNpv);
        }

        pnlList.Sort();

        int index = (int)Math.Floor((1 - confidenceLevel) * pnlList.Count);
        if (index < 0) index = 0;

        double varAmount = -pnlList[index];
        double expectedShortfall = -pnlList.Take(index + 1).Average();

        return new VaRResult
        {
            ConfidenceLevel = confidenceLevel,
            VaRAmount = varAmount,
            ExpectedShortfall = expectedShortfall,
            Method = "Monte Carlo Simulation"
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
