// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using ReactiveUI;
using Splat;

namespace CrissCross.WinForms;

/// <summary>Hosts WinForms navigation content.</summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="Form" />
/// <seealso cref="IViewFor&lt;TViewModel&gt;" />
public partial class NavigationForm<TViewModel> : NavigationForm, IViewFor<TViewModel>
where TViewModel : class, IRxObject, new()
{
    /// <summary>Initializes a new instance of the <see cref="NavigationForm{TViewModel}"/> class.</summary>
    public NavigationForm()
    {
        InitializeComponent();
        Load += OnLoad;
    }

    /// <inheritdoc/>
    [Category("CrissCross")]
    [Description("The ViewModel.")]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TViewModel? ViewModel { get; set; }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }

    /// <summary>Initializes the view model when the form loads.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="eventArgs">The event arguments.</param>
    private void OnLoad(object? sender, EventArgs eventArgs) =>
        ViewModel ??= AppLocator.Current.GetService<TViewModel>() ?? new();
}
