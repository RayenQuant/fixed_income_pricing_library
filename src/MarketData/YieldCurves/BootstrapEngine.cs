#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FixedIncomePricingLibrary.Analytics.DayCount;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;

namespace FixedIncomePricingLibrary.MarketData.YieldCurves;

/// <summary>
/// Solves for discount factors from market quotes via bootstrapping.
/// </summary>
public class BootstrapEngine
{
    public (double[] times, double[] dfs) Bootstrap(
        DateTime valuationDate,
        IEnumerable<(DateTime maturity, double rate)> instruments,
        DayCountConvention dayCount)
    {
        var sorted = instruments.OrderBy(i => i.maturity).ToList();
        var times = new List<double> { 0.0 };
        var dfs = new List<double> { 1.0 };

        var calculator = DayCountFactory.Create(dayCount);

        foreach (var inst in sorted)
        {
            double t = (inst.maturity - valuationDate).TotalDays / 365.0;
            double dcf = calculator.YearFraction(valuationDate, inst.maturity);

            // Basic bootstrapping for Deposit rates: DF = 1 / (1 + r * dcf)
            double df = 1.0 / (1.0 + inst.rate * dcf);

            times.Add(t);
            dfs.Add(df);
        }

        return (times.ToArray(), dfs.ToArray());
    }
}
