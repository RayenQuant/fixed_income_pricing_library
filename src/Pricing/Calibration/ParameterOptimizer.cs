#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;

namespace FixedIncomePricingLibrary.Pricing.Calibration;

/// <summary>
/// Wraps MathNet.Numerics optimization for model parameter calibration.
/// </summary>
public class ParameterOptimizer
{
    private readonly double _tolerance;
    private readonly int _maxIterations;

    public ParameterOptimizer(double tolerance = 1e-6, int maxIterations = 1000)
    {
        _tolerance = tolerance;
        _maxIterations = maxIterations;
    }

    public void Optimize(
        Func<double[], double> objectiveFunction,
        IList<ModelParameter> parameters)
    {
        var nonFixedParams = parameters.Where(p => !p.IsFixed).ToList();
        if (!nonFixedParams.Any()) return;

        // Initial guess
        var initialGuess = Vector<double>.Build.Dense(nonFixedParams.Select(p => p.Value).ToArray());

        // Define the objective function for MathNet
        // We use a wrapper to handle bounds (simple penalty or transformation)
        // For now, simple Nelder-Mead
        var obj = ObjectiveFunction.Value(x =>
        {
            // Apply bounds check (soft penalty)
            for (int i = 0; i < x.Count; i++)
            {
                if (x[i] < nonFixedParams[i].LowerBound || x[i] > nonFixedParams[i].UpperBound)
                    return 1e10; // Penalty
            }
            return objectiveFunction(x.ToArray());
        });

        var solver = new NelderMeadSimplex(_tolerance, _maxIterations);
        var result = solver.FindMinimum(obj, initialGuess);

        // Update parameter values
        for (int i = 0; i < nonFixedParams.Count; i++)
        {
            nonFixedParams[i].Value = result.MinimizingPoint[i];
        }
    }
}
