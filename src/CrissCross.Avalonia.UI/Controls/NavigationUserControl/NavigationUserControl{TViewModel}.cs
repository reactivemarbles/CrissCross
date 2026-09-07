// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using ReactiveUI;
using Splat;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>A navigation user control with a strongly typed view model.</summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
/// <seealso cref="NavigationUserControl" />
/// <seealso cref="IViewFor&lt;TViewModel&gt;" />
public class NavigationUserControl<TViewModel> : NavigationUserControl, IViewFor<TViewModel>
    where TViewModel : class, IRxObject, new()
{
    /// <summary>Gets the binding root view model.</summary>
    public TViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public new TViewModel? ViewModel
    {
        get => (TViewModel?)base.ViewModel;
        set => base.ViewModel = value;
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }

    /// <inheritdoc/>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ViewModel ??= AppLocator.Current.GetService<TViewModel>() ?? new();
    }
}
