// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators;

/// <summary>Generates ticks from explicitly supplied date values.</summary>
public class DateTimeManual : ITickGenerator
{
    /// <summary>The supplied ticks used to generate the visible tick list.</summary>
    private readonly List<Tick> _tickList = new();

    /// <summary>Initializes a new instance of the <see cref="DateTimeManual"/> class.</summary>
    public DateTimeManual()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DateTimeManual"/> class with specified ticks.</summary>
    /// <param name="ticks">The ticks to include.</param>
    public DateTimeManual(Tick[] ticks)
    {
        _tickList.AddRange(ticks);
    }

    /// <summary>Initializes a new instance of the <see cref="DateTimeManual"/> class with date positions and labels.</summary>
    /// <param name="positions">The tick positions.</param>
    /// <param name="labels">The corresponding tick labels.</param>
    public DateTimeManual(DateTime[] positions, string[] labels)
    {
        if (positions.Length != labels.Length)
        {
            throw new ArgumentException($"{nameof(positions)} must have same length as {nameof(labels)}");
        }

        for (int i = 0; i < positions.Length; i++)
        {
            AddMajor(positions[i], labels[i]);
        }
    }

    /// <summary>Gets the ticks visible within the most recently generated range.</summary>
    public Tick[] Ticks { get; private set; } = [];

    /// <summary>Gets or sets the maximum number of ticks.</summary>
    public int MaxTickCount { get; set; } = 50;

    /// <summary>Updates the visible ticks for the specified coordinate range.</summary>
    /// <param name="range">The visible coordinate range.</param>
    /// <param name="edge">The axis edge.</param>
    /// <param name="size">The axis size.</param>
    /// <param name="paint">The paint settings.</param>
    /// <param name="labelStyle">The tick-label style.</param>
    public void Regenerate(CoordinateRange range, Edge edge, PixelLength size, Paint paint, LabelStyle labelStyle)
    {
        List<Tick> visibleTicks = [];
        foreach (Tick tick in _tickList)
        {
            if (range.Contains(tick.Position))
            {
                visibleTicks.Add(tick);
            }
        }

        Ticks = [.. visibleTicks];
    }

    /// <summary>Adds a tick to the generator.</summary>
    /// <param name="tick">The tick to add.</param>
    public void Add(Tick tick) => _tickList.Add(tick);

    /// <summary>Adds a major tick at the specified date and label.</summary>
    /// <param name="position">The tick position.</param>
    /// <param name="label">The tick label.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2016",
        Justification = "DateTime is converted to an OLE Automation coordinate; changing this public signature would break compatibility.")]
    public void AddMajor(DateTime position, string label) => Add(Tick.Major(position.ToOADate(), label));

    /// <summary>Adds a minor tick at the specified date.</summary>
    /// <param name="position">The tick position.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2016",
        Justification = "DateTime is converted to an OLE Automation coordinate; changing this public signature would break compatibility.")]
    public void AddMinor(DateTime position) => Add(Tick.Minor(position.ToOADate()));
}
