// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Presents a consistent no-data, no-results, error, offline, or permission-required state.</summary>
public class EmptyState : System.Windows.Controls.ContentControl
{
    /// <summary>Property for <see cref="Model"/>.</summary>
    public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
        nameof(Model),
        typeof(EmptyStateModel),
        typeof(EmptyState),
        new PropertyMetadata(null));

    /// <summary>Gets or sets the empty-state model.</summary>
    public EmptyStateModel? Model
    {
        get => (EmptyStateModel?)GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }
}
