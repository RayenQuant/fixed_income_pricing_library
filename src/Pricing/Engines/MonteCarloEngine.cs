#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;
using FixedIncomePricingLibrary.Instruments;

namespace FixedIncomePricingLibrary.Pricing.Engines;

/// <summary>
/// Generic Monte Carlo pricing engine for interest rate derivatives.
/// </summary>
public class MonteCarloEngine : IPricingEngine
{
    private readonly int _simulations;
    private readonly IYieldCurve _curve;
    private readonly double _a;
    private readonly double _sigma;
    private readonly PathGenerator _pathGenerator = new();

    public MonteCarloEngine(IYieldCurve curve, double a, double sigma, int simulations = 10000)
    {
        _curve = curve;
        _a = a;
        _sigma = sigma;
        _simulations = simulations;
    }

    public PricingResult Price(IInstrument instrument)
    {
        double T = (instrument.MaturityDate - _curve.ValuationDate).TotalDays / 365.0;
        int steps = (int)(T * 252); // Daily steps
        if (steps < 10) steps = 10;

        double r0 = _curve.GetZeroRate(_curve.ValuationDate.AddDays(1));
        double theta = _curve.GetZeroRate(instrument.MaturityDate) * _a; // Simple approximation for drift

        double totalPayoff = 0;

        for (int i = 0; i < _simulations / 2; i++)
        {
            var (path, antiPath) = _pathGenerator.GenerateHullWhiteAntitheticPaths(r0, theta, _a, _sigma, T, steps);

            totalPayoff += CalculatePayoff(instrument, path);
            totalPayoff += CalculatePayoff(instrument, antiPath);
        }

        double pv = (totalPayoff / _simulations) * _curve.GetDiscountFactor(instrument.MaturityDate);

        return new PricingResult(instrument)
        {
            DirtyPrice = pv,
            CleanPrice = pv,
            Timestamp = DateTime.UtcNow
        };
    }

    private double CalculatePayoff(IInstrument instrument, double[] path)
    {
        // For a bond, the payoff is the discounted sum of remaining coupons + principal
        // In a path-based MC, we look at the final rate or the integrated path
        // Here we simplify: if it's a Bond, we return the terminal payoff (Principal + Final Coupon)
        // A more advanced engine would accumulate cash flows along the path.

        if (instrument is Bond bond)
        {
            return bond.Notional * (1.0 + bond.CouponRate / (int)bond.Frequency);
        }

        return instrument.Notional; // Default for ZCB or other simple instruments
    }
}
