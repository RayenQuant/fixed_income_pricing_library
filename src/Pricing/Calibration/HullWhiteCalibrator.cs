#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Pricing.Engines;

namespace FixedIncomePricingLibrary.Pricing.Calibration;

/// <summary>
/// Calibrator for the Hull-White model using market swaptions or bonds.
/// </summary>
public class HullWhiteCalibrator : IModelCalibrator
{
    private readonly IYieldCurve _curve;
    public List<ModelParameter> Parameters { get; } = new();

    public HullWhiteCalibrator(IYieldCurve curve)
    {
        _curve = curve;
        Parameters.Add(new ModelParameter { Name = "MeanReversion", Value = 0.03, LowerBound = 0.001, UpperBound = 0.5 });
        Parameters.Add(new ModelParameter { Name = "Volatility", Value = 0.01, LowerBound = 0.0001, UpperBound = 0.1 });
    }

    public void Calibrate(IEnumerable<IInstrument> marketInstruments, IEnumerable<double> marketPrices)
    {
        var instruments = marketInstruments.ToList();
        var prices = marketPrices.ToList();

        if (instruments.Count != prices.Count)
            throw new ArgumentException("Number of instruments must match number of market prices.");

        var optimizer = new ParameterOptimizer();

        optimizer.Optimize(x =>
        {
            double a = x[0];
            double sigma = x[1];

            var engine = new HullWhiteEngine(_curve, a, sigma);
            var modelPrices = instruments.Select(inst => engine.Price(inst).DirtyPrice).ToList();

            return ObjectiveFunctions.CalculateRMSE(modelPrices, prices);
        }, Parameters);
    }
}
