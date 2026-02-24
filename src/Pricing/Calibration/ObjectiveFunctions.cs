#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace FixedIncomePricingLibrary.Pricing.Calibration;

/// <summary>
/// Providing common objective functions for calibration.
/// </summary>
public static class ObjectiveFunctions
{
    /// <summary>
    /// Calculates Root Mean Square Error (RMSE).
    /// </summary>
    public static double CalculateRMSE(IEnumerable<double> modelValues, IEnumerable<double> marketValues)
    {
        var modelArr = modelValues.ToArray();
        var marketArr = marketValues.ToArray();

        if (modelArr.Length != marketArr.Length)
            throw new ArgumentException("Model and Market value arrays must have the same length.");

        double sumSq = 0;
        for (int i = 0; i < modelArr.Length; i++)
        {
            double diff = modelArr[i] - marketArr[i];
            sumSq += diff * diff;
        }

        return Math.Sqrt(sumSq / modelArr.Length);
    }
}
