#nullable enable
using System;
using MathNet.Numerics.Distributions;

namespace FixedIncomePricingLibrary.Pricing.Engines;

/// <summary>
/// Generates stochastic interest rate paths using various models.
/// </summary>
public class PathGenerator
{
    private readonly Random _random = new();

    /// <summary>
    /// Generates a path under Hull-White dynamics: dr = [theta - a*r]dt + sigma*dW
    /// </summary>
    public double[] GenerateHullWhitePath(double r0, double theta, double a, double sigma, double T, int steps)
    {
        double dt = T / steps;
        var path = new double[steps + 1];
        path[0] = r0;

        for (int i = 1; i <= steps; i++)
        {
            double dz = Normal.Sample(_random, 0, 1) * Math.Sqrt(dt);
            path[i] = path[i - 1] + (theta - a * path[i - 1]) * dt + sigma * dz;
        }

        return path;
    }

    /// <summary>
    /// Generates a pair of paths using antithetic variates for variance reduction.
    /// </summary>
    public (double[] path, double[] antitheticPath) GenerateHullWhiteAntitheticPaths(double r0, double theta, double a, double sigma, double T, int steps)
    {
        double dt = T / steps;
        var path = new double[steps + 1];
        var antiPath = new double[steps + 1];
        path[0] = r0;
        antiPath[0] = r0;

        for (int i = 1; i <= steps; i++)
        {
            double epsilon = Normal.Sample(_random, 0, 1);
            double dz = epsilon * Math.Sqrt(dt);
            double dzAnti = -epsilon * Math.Sqrt(dt);

            path[i] = path[i - 1] + (theta - a * path[i - 1]) * dt + sigma * dz;
            antiPath[i] = antiPath[i - 1] + (theta - a * antiPath[i - 1]) * dt + sigma * dzAnti;
        }

        return (path, antiPath);
    }
}
