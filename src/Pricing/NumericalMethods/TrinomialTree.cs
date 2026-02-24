#nullable enable
using System;
using System.Collections.Generic;

namespace FixedIncomePricingLibrary.Pricing.NumericalMethods;

/// <summary>
/// Represents a generic trinomial tree structure for numerical pricing.
/// </summary>
public class TrinomialTree
{
    private readonly double[][] _values;
    private readonly int _steps;

    public TrinomialTree(int steps)
    {
        _steps = steps;
        _values = new double[steps + 1][];
        for (int i = 0; i <= steps; i++)
        {
            _values[i] = new double[2 * i + 1];
        }
    }

    /// <summary>
    /// Gets or sets the value at a specific node (step, index).
    /// Index ranges from 0 to 2*step.
    /// </summary>
    public double this[int step, int index]
    {
        get => _values[step][index];
        set => _values[step][index] = value;
    }

    public int Steps => _steps;

    /// <summary>
    /// Performs backward induction to calculate the value at the root node.
    /// </summary>
    /// <param name="payoffFunc">Function to calculate the payoff at each node.</param>
    /// <param name="discountFunc">Function to calculate the discount factor for a node transition.</param>
    /// <param name="probsFunc">Function to calculate (p_up, p_mid, p_down) probabilities for a node.</param>
    public double BackwardInduction(
        Func<int, int, double, double> nodeProcessor)
    {
        // This is a template. Specific implementations (Hull-White) will drive the logic.
        return _values[0][0];
    }
}
