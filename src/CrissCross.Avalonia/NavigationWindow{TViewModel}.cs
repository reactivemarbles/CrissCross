// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using ReactiveUI;
using Splat;

namespace CrissCross.Avalonia;

/// <summary>Navigation Window.</summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="NavigationWindow" />
/// <seealso cref="IViewFor&lt;TViewModel&gt;" />
public class NavigationWindow<TViewModel> : NavigationWindow, IViewFor<TViewModel>
    where TViewModel : class, IRxObject, new()
{
    /// <summary>The view model dependency property.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1002", Justification = "Generic avalonia property is expected here.")]
    public static readonly StyledProperty<TViewModel?> ViewModelProperty = AvaloniaProperty
        .Register<NavigationWindow<TViewModel>, TViewModel?>(nameof(ViewModel));

    /// <summary>Initializes a new instance of the <see cref="NavigationWindow{TViewModel}"/> class.</summary>
    public NavigationWindow() => AttachedToVisualTree += OnAttachedToVisualTree;

    /// <summary>Gets the binding root view model.</summary>
    public TViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }

    /// <summary>Initializes the view model when the window enters a visual tree.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="eventArgs">The visual-tree attachment event arguments.</param>
    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs eventArgs) =>
        ViewModel ??= AppLocator.Current.GetService<TViewModel>() ?? new();
}
