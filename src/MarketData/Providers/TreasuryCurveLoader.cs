#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using FixedIncomePricingLibrary.Core.Enums;
using FixedIncomePricingLibrary.Core.Interfaces;
using FixedIncomePricingLibrary.MarketData.YieldCurves;

namespace FixedIncomePricingLibrary.MarketData.Providers;

public static class TreasuryCurveLoader
{
    public static IYieldCurve Load(string filePath, DateTime valuationDate)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        var records = csv.GetRecords<dynamic>().ToList();
        var row = records.First(r => DateTime.Parse((string)r.Date, CultureInfo.InvariantCulture) == valuationDate.Date);

        var quotes = new List<(DateTime, double)>();
        var dict = (IDictionary<string, object>)row;

        foreach (var kvp in dict)
        {
            if (kvp.Key == "Date") continue;

            var tenor = kvp.Key;
            var rate = double.Parse((string)kvp.Value, CultureInfo.InvariantCulture) / 100.0;
            var maturity = ParseTenor(valuationDate, tenor);
            quotes.Add((maturity, rate));
        }

        return YieldCurveBuilder.Build(valuationDate, quotes);
    }

    private static DateTime ParseTenor(DateTime valDate, string tenor)
    {
        return tenor switch
        {
            "1M" => valDate.AddMonths(1),
            "3M" => valDate.AddMonths(3),
            "6M" => valDate.AddMonths(6),
            "1Y" => valDate.AddYears(1),
            "2Y" => valDate.AddYears(2),
            "3Y" => valDate.AddYears(3),
            "5Y" => valDate.AddYears(5),
            "7Y" => valDate.AddYears(7),
            "10Y" => valDate.AddYears(10),
            "20Y" => valDate.AddYears(20),
            "30Y" => valDate.AddYears(30),
            _ => throw new ArgumentException($"Unknown tenor: {tenor}")
        };
    }
}
