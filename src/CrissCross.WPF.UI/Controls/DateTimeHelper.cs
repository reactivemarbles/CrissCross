// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// NOTICE: This date time helper assumes it is working in a Gregorian calendar
/// If we ever support non Gregorian calendars this class would need to be redesigned.
/// </summary>
internal static class DateTimeHelper
{
    /// <summary>The number of months in a year.</summary>
    private const int MonthsInYear = 12;

    /// <summary>The number of years in a decade.</summary>
    private const int YearsInDecade = 10;

    /// <summary>The offset from the start of a decade to its final year.</summary>
    private const int DecadeEndOffset = 9;

    /// <summary>The offset used to convert one-based month numbers to zero-based indexes.</summary>
    private const int MonthIndexOffset = 1;

    /// <summary>Provides the cal member.</summary>
    private static readonly System.Globalization.Calendar cal = new GregorianCalendar();

    /// <summary>Provides the AddDays member.</summary>
    /// <param name="time">The time value.</param>
    /// <param name="days">The days value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the AddMonths member.</summary>
    /// <param name="time">The time value.</param>
    /// <param name="months">The months value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the AddYears member.</summary>
    /// <param name="time">The time value.</param>
    /// <param name="years">The years value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the SetYear member.</summary>
    /// <param name="date">The date value.</param>
    /// <param name="year">The year value.</param>
    /// <returns>The result.</returns>
    public static DateTime? SetYear(DateTime date, int year) => AddYears(date, year - date.Year);

    /// <summary>Provides the SetYearMonth member.</summary>
    /// <param name="date">The date value.</param>
    /// <param name="yearMonth">The yearMonth value.</param>
    /// <returns>The result.</returns>
    public static DateTime? SetYearMonth(DateTime date, DateTime yearMonth)
    {
        var target = SetYear(date, yearMonth.Year);
        if (target.HasValue)
        {
            target = AddMonths(target.Value, yearMonth.Month - date.Month);
        }

        return target;
    }

    /// <summary>Provides the CompareDays member.</summary>
    /// <param name="dt1">The dt1 value.</param>
    /// <param name="dt2">The dt2 value.</param>
    /// <returns>The result.</returns>
    public static int CompareDays(DateTime dt1, DateTime dt2) =>
        DateTime.Compare(DiscardTime(dt1)!.Value, DiscardTime(dt2)!.Value);

    /// <summary>Provides the CompareYearMonth member.</summary>
    /// <param name="dt1">The dt1 value.</param>
    /// <param name="dt2">The dt2 value.</param>
    /// <returns>The result.</returns>
    public static int CompareYearMonth(DateTime dt1, DateTime dt2) =>
        ((dt1.Year - dt2.Year) * MonthsInYear) + (dt1.Month - dt2.Month);

    /// <summary>Provides the DecadeOfDate member.</summary>
    /// <param name="date">The date value.</param>
    /// <returns>The result.</returns>
    public static int DecadeOfDate(DateTime date) => date.Year - (date.Year % YearsInDecade);

    /// <summary>Provides the DiscardDayTime member.</summary>
    /// <param name="d">The d value.</param>
    /// <returns>The result.</returns>
    public static DateTime DiscardDayTime(DateTime d) => new(d.Year, d.Month, 1, 0, 0, 0, d.Kind);

    /// <summary>Provides the DiscardTime member.</summary>
    /// <param name="d">The d value.</param>
    /// <returns>The result.</returns>
    public static DateTime? DiscardTime(DateTime? d) =>
        d switch
        {
            null => null,
            _ => new DateTime(d.Value.Year, d.Value.Month, d.Value.Day, 0, 0, 0, d.Value.Kind),
        };

    /// <summary>Provides the EndOfDecade member.</summary>
    /// <param name="date">The date value.</param>
    /// <returns>The result.</returns>
    public static int EndOfDecade(DateTime date) => DecadeOfDate(date) + DecadeEndOffset;

    /// <summary>Provides the GetCurrentDateFormat member.</summary>
    /// <returns>The result.</returns>
    public static DateTimeFormatInfo GetCurrentDateFormat() => GetDateFormat(CultureInfo.CurrentCulture);

    // returns if the date is included in the range
    /// <summary>Ins the range.</summary>
    /// <param name="date">The date.</param>
    /// <param name="range">The range.</param>
    /// <returns>A bool.</returns>
    public static bool InRange(DateTime date, CalendarDateRange range) => InRange(date, range.Start, range.End);

    /// <summary>Returns if the date is included in the range.</summary>
    /// <param name="date">The date value.</param>
    /// <param name="start">The start value.</param>
    /// <param name="end">The end value.</param>
    /// <returns><c>true</c> if the date is within the range; otherwise, <c>false</c>.</returns>
    public static bool InRange(DateTime date, DateTime start, DateTime end)
    {
        Debug.Assert(DateTime.Compare(start, end) < 1, "Less than 1");

        return CompareDays(date, start) > -1 && CompareDays(date, end) < 1;
    }

    /// <summary>Provides the ToDayString member.</summary>
    /// <param name="date">The date value.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the ToYearMonthPatternString member.</summary>
    /// <param name="date">The date value.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public static string ToYearMonthPatternString(DateTime? date, CultureInfo culture)
    {
        var result = string.Empty;
        var format = GetDateFormat(culture);

        if (date.HasValue && format is not null)
        {
            result = date.Value.ToString(format.YearMonthPattern, format);
        }

        return result;
    }

    /// <summary>Provides the ToYearString member.</summary>
    /// <param name="date">The date value.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public static string ToYearString(DateTime? date, CultureInfo culture)
    {
        var result = string.Empty;
        var format = GetDateFormat(culture);

        if (date.HasValue && format is not null)
        {
            result = date.Value.Year.ToString(format);
        }

        return result;
    }

    /// <summary>Provides the ToAbbreviatedMonthString member.</summary>
    /// <param name="date">The date value.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public static string ToAbbreviatedMonthString(DateTime? date, CultureInfo culture)
    {
        var result = string.Empty;
        var format = GetDateFormat(culture);

        if (date.HasValue && format is not null)
        {
            var monthNames = format.AbbreviatedMonthNames;

            if (monthNames?.Length > 0)
            {
                result = monthNames[(date.Value.Month - MonthIndexOffset) % monthNames.Length];
            }
        }

        return result;
    }

    /// <summary>Provides the ToLongDateString member.</summary>
    /// <param name="date">The date value.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public static string ToLongDateString(DateTime? date, CultureInfo culture)
    {
        var result = string.Empty;
        var format = GetDateFormat(culture);

        if (date.HasValue && format is not null)
        {
            result = date.Value.ToString(format.LongDatePattern, format);
        }

        return result;
    }

    /// <summary>Provides the GetDateFormat member.</summary>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    internal static DateTimeFormatInfo GetDateFormat(CultureInfo culture)
    {
        if (culture.Calendar is GregorianCalendar)
        {
            return culture.DateTimeFormat;
        }

        GregorianCalendar? foundCal = default;
        foreach (var optionalCalendar in culture.OptionalCalendars)
        {
            if (optionalCalendar is not GregorianCalendar)
            {
                continue;
            }

            // Return the first Gregorian calendar with CalendarType == Localized
            // Otherwise return the first Gregorian calendar
            foundCal ??= optionalCalendar as GregorianCalendar;

            if (((GregorianCalendar)cal).CalendarType == GregorianCalendarTypes.Localized)
            {
                foundCal = cal as GregorianCalendar;

                break;
            }
        }

        DateTimeFormatInfo dtfi;
        if (foundCal is null)
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
