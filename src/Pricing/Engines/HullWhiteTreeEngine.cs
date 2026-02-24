#nullable enable
using System;
using System.Collections.Generic;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.Core.Models;
using FixedIncomePricingLibrary.Instruments;
using FixedIncomePricingLibrary.Pricing.NumericalMethods;

namespace FixedIncomePricingLibrary.Pricing.Engines;

/// <summary>
/// Implements pricing for interest rate derivatives using a Trinomial Tree under Hull-White model.
/// </summary>
public class HullWhiteTreeEngine : IPricingEngine
{
    private readonly IYieldCurve _curve;
    private readonly double _a;
    private readonly double _sigma;
    private readonly int _steps;

    public HullWhiteTreeEngine(IYieldCurve curve, double a, double sigma, int steps = 50)
    {
        _curve = curve;
        _a = a;
        _sigma = sigma;
        _steps = steps;
    }

    public PricingResult Price(IInstrument instrument)
    {
        double T = (instrument.MaturityDate - _curve.ValuationDate).TotalDays / 365.0;
        if (T <= 0) T = 0.0001;

        double dt = T / _steps;
        double dx = _sigma * Math.Sqrt(3 * dt);

        // 1. Build the tree for X (process with zero mean)
        var tree = new TrinomialTree(_steps);

        // 2. Assign values at maturity (Payoff)
        for (int j = 0; j <= 2 * _steps; j++)
        {
            tree[_steps, j] = CalculatePayoff(instrument, j, dx);
        }

        // 3. Backward Induction
        for (int i = _steps - 1; i >= 0; i--)
        {
            for (int j = 0; j <= 2 * i; j++)
            {
                var (pu, pm, pd) = GetProbabilities(i, j - i, dt, dx);

                // Index mapping: j is index in level i. 
                // Successors at i+1 are centered around j+1 (due to 2*step+1 width)
                double val = pu * tree[i + 1, j + 2] + pm * tree[i + 1, j + 1] + pd * tree[i + 1, j];

                // Discounting
                double r = GetShortRate(i, j - i, dt, dx);
                tree[i, j] = val * Math.Exp(-r * dt);

                // Early exercise check (for American/Bermudan)
                double exerciseValue = CalculateExerciseValue(instrument, i, j, dt, dx);
                tree[i, j] = Math.Max(tree[i, j], exerciseValue);
            }
        }

        return new PricingResult(instrument)
        {
            DirtyPrice = tree[0, 0],
            CleanPrice = tree[0, 0],
            Timestamp = DateTime.UtcNow
        };
    }

    private double GetShortRate(int step, int j, double dt, double dx)
    {
        double alpha = _curve.GetZeroRate(_curve.ValuationDate.AddDays(step * dt * 365)) + 0.01; // Simple alpha(t) fit
        return alpha + j * dx;
    }

    private (double pu, double pm, double pd) GetProbabilities(int step, int j, double dt, double dx)
    {
        // Standard Hull-White branching probabilities
        double nu = -_a * j * dx * dt;
        double dx2 = dx * dx;

        double pu = 1.0 / 6.0 + (nu * nu / (2 * dx2)) + (nu / (2 * dx));
        double pm = 2.0 / 3.0 - (nu * nu / dx2);
        double pd = 1.0 / 6.0 + (nu * nu / (2 * dx2)) - (nu / (2 * dx));

        return (pu, pm, pd);
    }

    private double CalculatePayoff(IInstrument instrument, int j, double dx)
    {
        // Placeholder: For a ZCB, payoff is 1.0 at maturity.
        return 1.0;
    }

    private double CalculateExerciseValue(IInstrument instrument, int step, int j, double dt, double dx)
    {
        // Placeholder: No early exercise for standard engine demo.
        return 0;
    }
}
