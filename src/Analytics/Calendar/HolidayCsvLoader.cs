#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace FixedIncomePricingLibrary.Analytics.Calendar;

/// <summary>
/// Loads holidays from a CSV file.
/// </summary>
public static class HolidayCsvLoader
{
    public static IEnumerable<DateTime> Load(string filePath)
    {
        if (!File.Exists(filePath))
            return Array.Empty<DateTime>();

        var holidays = new List<DateTime>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            var dateStr = csv.GetField("Date");
            if (DateTime.TryParse(dateStr, out DateTime date))
            {
                holidays.Add(date.Date);
            }
        }

        return holidays;
    }
}
