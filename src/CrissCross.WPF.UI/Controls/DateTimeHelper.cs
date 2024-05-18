// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

// NOTICE: This date time helper assumes it is working in a Gregorian calendar
//         If we ever support non Gregorian calendars this class would need to be redesigned
internal static class DateTimeHelper
{
    private static readonly System.Globalization.Calendar cal = new GregorianCalendar();

    public static DateTime? AddDays(DateTime time, int days)
    {
        try
        {
            return cal.AddDays(time, days);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public static DateTime? AddMonths(DateTime time, int months)
    {
        try
        {
            return cal.AddMonths(time, months);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public static DateTime? AddYears(DateTime time, int years)
    {
        try
        {
            return cal.AddYears(time, years);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public static DateTime? SetYear(DateTime date, int year) => AddYears(date, year - date.Year);

    public static DateTime? SetYearMonth(DateTime date, DateTime yearMonth)
    {
        var target = SetYear(date, yearMonth.Year);
        if (target.HasValue)
        {
            target = AddMonths(target.Value, yearMonth.Month - date.Month);
        }

        return target;
    }

    public static int CompareDays(DateTime dt1, DateTime dt2) =>
        DateTime.Compare(DiscardTime(dt1)!.Value, DiscardTime(dt2)!.Value);

    public static int CompareYearMonth(DateTime dt1, DateTime dt2) =>
        ((dt1.Year - dt2.Year) * 12) + (dt1.Month - dt2.Month);

    public static int DecadeOfDate(DateTime date) => date.Year - (date.Year % 10);

    public static DateTime DiscardDayTime(DateTime d) => new(d.Year, d.Month, 1, 0, 0, 0);

    public static DateTime? DiscardTime(DateTime? d) => d switch
    {
        null => null,
        _ => d.Value.Date
    };

    public static int EndOfDecade(DateTime date) => DecadeOfDate(date) + 9;

    public static DateTimeFormatInfo GetCurrentDateFormat() => GetDateFormat(CultureInfo.CurrentCulture);

    // returns if the date is included in the range
    /// <summary>
    /// Ins the range.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="range">The range.</param>
    /// <returns>A bool.</returns>
    public static bool InRange(DateTime date, CalendarDateRange range) => InRange(date, range.Start, range.End);

    // returns if the date is included in the range
    public static bool InRange(DateTime date, DateTime start, DateTime end)
    {
        Debug.Assert(DateTime.Compare(start, end) < 1, "Less than 1");

        return CompareDays(date, start) > -1 && CompareDays(date, end) < 1;
    }

    public static string ToDayString(DateTime? date, CultureInfo culture)
    {
        var result = string.Empty;
        var format = GetDateFormat(culture);

        if (date.HasValue && format is not null)
        {
            result = date.Value.Day.ToString(format);
        }

        return result;
    }

    public static string ToYearMonthPatternString(DateTime? date, CultureInfo culture)
    {
        var result = string.Empty;
        var format = GetDateFormat(culture);

        if (date.HasValue && format != null)
        {
            result = date.Value.ToString(format.YearMonthPattern, format);
        }

        return result;
    }

    public static string ToYearString(DateTime? date, CultureInfo culture)
    {
        var result = string.Empty;
        var format = GetDateFormat(culture);

        if (date.HasValue && format != null)
        {
            result = date.Value.Year.ToString(format);
        }

        return result;
    }

    public static string ToAbbreviatedMonthString(DateTime? date, CultureInfo culture)
    {
        var result = string.Empty;
        var format = GetDateFormat(culture);

        if (date.HasValue && format is not null)
        {
            var monthNames = format.AbbreviatedMonthNames;

            if (monthNames?.Length > 0)
            {
                result = monthNames[(date.Value.Month - 1) % monthNames.Length];
            }
        }

        return result;
    }

    public static string ToLongDateString(DateTime? date, CultureInfo culture)
    {
        var result = string.Empty;
        var format = GetDateFormat(culture);

        if (date.HasValue && format is not null)
        {
            result = date.Value.Date.ToString(format.LongDatePattern, format);
        }

        return result;
    }

    internal static DateTimeFormatInfo GetDateFormat(CultureInfo culture)
    {
        if (culture.Calendar is GregorianCalendar)
        {
            return culture.DateTimeFormat;
        }

        GregorianCalendar? foundCal = default;
        foreach (var cal in culture.OptionalCalendars)
        {
            if (cal is not GregorianCalendar)
            {
                continue;
            }

            // Return the first Gregorian calendar with CalendarType == Localized
            // Otherwise return the first Gregorian calendar
            foundCal ??= cal as GregorianCalendar;

            if (((GregorianCalendar)cal).CalendarType == GregorianCalendarTypes.Localized)
            {
                foundCal = cal as GregorianCalendar;

                break;
            }
        }

        DateTimeFormatInfo dtfi;
        if (foundCal == null)
        {
            // if there are no GregorianCalendars in the OptionalCalendars list, use the invariant dtfi
            dtfi = ((CultureInfo)CultureInfo.InvariantCulture.Clone()).DateTimeFormat;
            dtfi.Calendar = new GregorianCalendar();
        }
        else
        {
            dtfi = ((CultureInfo)culture.Clone()).DateTimeFormat;
            dtfi.Calendar = foundCal;
        }

        return dtfi;
    }
}
