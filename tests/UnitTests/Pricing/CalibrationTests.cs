using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Instruments;
using FixedIncomePricingLibrary.MarketData.YieldCurves;
using FixedIncomePricingLibrary.Pricing.Calibration;
using FixedIncomePricingLibrary.Pricing.Engines;
using Xunit;

namespace FixedIncomePricingLibrary.UnitTests.Pricing;

public class CalibrationTests
{
    [Fact]
    public void HullWhiteCalibrator_ShouldRecoverKnownParameters()
    {
        var valDate = DateTime.Today;
        var curve = YieldCurveBuilder.FlatCurve(valDate, 0.05);

        // Target parameters
        double targetA = 0.05;
        double targetSigma = 0.015;

        // Create synthetic market instruments (Swations represented as generic instruments for now)
        var instruments = new List<IInstrument>
        {
            new Bond { Id = "M1", Notional = 1000, MaturityDate = valDate.AddYears(1) },
            new Bond { Id = "M2", Notional = 1000, MaturityDate = valDate.AddYears(5) }
        };

        // Generate "market" prices using target parameters
        var targetEngine = new HullWhiteEngine(curve, targetA, targetSigma);
        var marketPrices = instruments.Select(inst => targetEngine.Price(inst).DirtyPrice).ToList();

        // Calibrate
        var calibrator = new HullWhiteCalibrator(curve);
        calibrator.Calibrate(instruments, marketPrices);

        double recoveredA = calibrator.Parameters.First(p => p.Name == "MeanReversion").Value;
        double recoveredSigma = calibrator.Parameters.First(p => p.Name == "Volatility").Value;

        // Check if recovered parameters are close to targets
        Assert.True(Math.Abs(recoveredA - targetA) < 0.01, $"A mismatch: {recoveredA} vs {targetA}");
        Assert.True(Math.Abs(recoveredSigma - targetSigma) < 0.005, $"Sigma mismatch: {recoveredSigma} vs {targetSigma}");
    }
}
