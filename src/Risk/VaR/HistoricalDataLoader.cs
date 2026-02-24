#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using FixedIncomePricingLibrary.Core.Exceptions;

namespace FixedIncomePricingLibrary.Risk.VaR;

/// <summary>
/// Loads historical yield curve data for VaR calculations.
/// </summary>
public class HistoricalDataLoader
{
    private readonly string _historicalDataPath;

    public HistoricalDataLoader(string historicalDataPath)
    {
        _historicalDataPath = historicalDataPath;
    }

    /// <summary>
    /// Loads historical rates from a CSV file.
    /// Expected format: Date, 1M, 3M, 6M, 1Y, 2Y, 5Y, 10Y, 30Y
    /// </summary>
    public IEnumerable<HistoricalRateEntry> LoadHistoricalRates()
    {
        if (!File.Exists(_historicalDataPath))
            throw new MarketDataException($"Historical data file not found: {_historicalDataPath}");

        using var reader = new StreamReader(_historicalDataPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null
        });

        return csv.GetRecords<HistoricalRateEntry>().ToList();
    }
}

public class HistoricalRateEntry
{
    public DateTime Date { get; set; }
    public double Rate1M { get; set; }
    public double Rate3M { get; set; }
    public double Rate6M { get; set; }
    public double Rate1Y { get; set; }
    public double Rate2Y { get; set; }
    public double Rate5Y { get; set; }
    public double Rate10Y { get; set; }
    public double Rate30Y { get; set; }
}
