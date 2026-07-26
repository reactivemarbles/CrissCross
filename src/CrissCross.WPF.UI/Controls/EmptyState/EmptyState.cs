// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Presents a consistent no-data, no-results, error, offline, or permission-required state.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class EmptyState : System.Windows.Controls.ContentControl
{
    /// <summary>Property for <see cref="Model"/>.</summary>
    public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
        nameof(Model),
        typeof(EmptyStateModel),
        typeof(EmptyState),
        new(null));

    /// <summary>Gets or sets the empty-state model.</summary>
    public EmptyStateModel? Model
    {
        get => (EmptyStateModel?)GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
