// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Represents platform-neutral state for a descriptor-driven data filter panel.</summary>
public sealed class DataFilterPanelState
{
    /// <summary>Formats the active-filter summary text.</summary>
#if NET8_0_OR_GREATER
    private static readonly System.Text.CompositeFormat SummaryFormat = System.Text.CompositeFormat.Parse("{0} active filters");
#else
    private const string SummaryFormat = "{0} active filters";
#endif

    /// <inheritdoc />
    public DataFilterPanelState()
        : this(null, null, false, false) { }

    /// <inheritdoc />
    public DataFilterPanelState(IReadOnlyList<FilterDescriptor>? descriptors)
        : this(descriptors, null, false, false) { }

    /// <inheritdoc />
    public DataFilterPanelState(
        IReadOnlyList<FilterDescriptor>? descriptors,
        IReadOnlyList<FilterExpression>? expressions)
        : this(descriptors, expressions, false, false) { }

    /// <inheritdoc />
    public DataFilterPanelState(
        IReadOnlyList<FilterDescriptor>? descriptors,
        IReadOnlyList<FilterExpression>? expressions,
        bool isDirty)
        : this(descriptors, expressions, isDirty, false) { }

    /// <summary>Initializes a new instance of the <see cref="DataFilterPanelState"/> class.</summary>
    /// <param name="descriptors">The available filter descriptors.</param>
    /// <param name="expressions">The current filter expressions.</param>
    /// <param name="isDirty">A value indicating whether edits are pending apply.</param>
    /// <param name="isApplying">A value indicating whether an apply operation is active.</param>
    public DataFilterPanelState(
        IReadOnlyList<FilterDescriptor>? descriptors,
        IReadOnlyList<FilterExpression>? expressions,
        bool isDirty,
        bool isApplying)
    {
        Descriptors = descriptors ?? [];
        Expressions = expressions ?? [];
        IsDirty = isDirty;
        IsApplying = isApplying;
        var activeExpressions = new List<FilterExpression>();
        var activeTokens = new List<FilterToken>();
        foreach (var expression in Expressions)
        {
            if (!expression.IsActive)
            {
                continue;
            }

            activeExpressions.Add(expression);
            activeTokens.Add(expression.ToToken(GetDescriptor(expression.FieldKey)));
        }

        ActiveExpressions = activeExpressions;
        ActiveTokens = activeTokens;
    }

    /// <summary>Gets the available filter descriptors.</summary>
    public IReadOnlyList<FilterDescriptor> Descriptors { get; }

    /// <summary>Gets the current filter expressions.</summary>
    public IReadOnlyList<FilterExpression> Expressions { get; }

    /// <summary>Gets a value indicating whether edits are pending apply.</summary>
    public bool IsDirty { get; }

    /// <summary>Gets a value indicating whether an apply operation is active.</summary>
    public bool IsApplying { get; }

    /// <summary>Gets the expressions that currently participate in filtering.</summary>
    public IReadOnlyList<FilterExpression> ActiveExpressions { get; }

    /// <summary>Gets the tokens projected from active expressions.</summary>
    public IReadOnlyList<FilterToken> ActiveTokens { get; }

    /// <summary>Gets the number of available descriptors.</summary>
    public int DescriptorCount => Descriptors.Count;

    /// <summary>Gets the number of active filters.</summary>
    public int ActiveFilterCount => ActiveExpressions.Count;

    /// <summary>Gets a value indicating whether the panel can apply pending edits.</summary>
    public bool CanApply => IsDirty && !IsApplying;

    /// <summary>Gets a value indicating whether the panel can clear active filters.</summary>
    public bool CanClear => ActiveFilterCount > 0 && !IsApplying;

    /// <summary>Gets a compact active-filter summary.</summary>
    public string SummaryText =>
        ActiveFilterCount switch
        {
            0 => "No filters",
            1 => "1 active filter",
            var count => string.Format(System.Globalization.CultureInfo.InvariantCulture, SummaryFormat, count),
        };

    /// <summary>Finds a descriptor by stable key.</summary>
    /// <param name="key">The descriptor key.</param>
    /// <returns>The descriptor when present; otherwise, <c>null</c>.</returns>
    public FilterDescriptor? GetDescriptor(string key)
    {
        foreach (var descriptor in Descriptors)
        {
            if (descriptor.Key == key)
            {
                return descriptor;
            }
        }

        return null;
    }

    /// <summary>Projects the active panel expressions into a search query state.</summary>
    /// <returns>The projected search query state.</returns>
    public SearchQueryState ToSearchQueryState() => ToSearchQueryState(null, null);

    /// <summary>Projects the active panel expressions and query text into a search query state.</summary>
    /// <param name="text">The optional query text.</param>
    /// <returns>The projected search query state.</returns>
    public SearchQueryState ToSearchQueryState(string? text) => ToSearchQueryState(text, null);

    /// <summary>Projects the active panel expressions into a search query state.</summary>
    /// <param name="text">The optional query text.</param>
    /// <param name="resultCount">The optional result count.</param>
    /// <returns>The projected search query state.</returns>
    public SearchQueryState ToSearchQueryState(string? text, int? resultCount) =>
        new(text, submittedText: text, resultCount: resultCount, filters: ActiveTokens);
}
