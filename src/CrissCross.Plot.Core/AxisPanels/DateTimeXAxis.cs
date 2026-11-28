// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.TickGenerators;

namespace CrissCross.Plot.AxisPanels;

/// <summary>Represents a horizontal axis with date and time coordinates.</summary>
public class DateTimeXAxis : XAxisBase
{
    /// <inheritdoc />
    public override Edge Edge { get; } = Edge.Bottom;

    /// <summary>Gets or sets the date/time tick generator for this axis.</summary>
    /// <exception cref="ArgumentException">The assigned generator does not generate date/time ticks.</exception>
    public override ITickGenerator TickGenerator
    {
        get => field;
        set
        {
            if (value is not IDateTimeTickGenerator)
            {
                throw new ArgumentException($"Date axis must have a {nameof(ITickGenerator)} generator");
            }

            field = (IDateTimeTickGenerator)value;
        }
    }
= new DateTimeAutomatic();

    /// <summary>Converts dates to the coordinate values used by this axis.</summary>
    /// <param name="dates">The dates to convert.</param>
    /// <returns>The corresponding coordinate values.</returns>
    public IEnumerable<double> ConvertToCoordinateSpace(IEnumerable<DateTime> dates) =>
        TickGenerator is IDateTimeTickGenerator dateTickGenerator
            ? dateTickGenerator.ConvertToCoordinateSpace(dates)
            : throw new InvalidOperationException("Date axis configured with non-date tick generator");
}
