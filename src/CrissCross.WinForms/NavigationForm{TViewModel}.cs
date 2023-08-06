// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using ReactiveUI;
using Splat;

namespace CrissCross.WinForms;

/// <summary>
/// NavigationForm.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="Form" />
/// <seealso cref="IViewFor&lt;TViewModel&gt;" />
public partial class NavigationForm<TViewModel> : NavigationForm, IViewFor<TViewModel>
where TViewModel : class, IRxObject, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationForm{TViewModel}"/> class.
    /// </summary>
    public NavigationForm()
    {
        InitializeComponent();
        this.WhenActivated(_ => ViewModel ??= Locator.Current.GetService<TViewModel>() ?? new());
    }

    /// <inheritdoc/>
    [Category("ReactiveMarbles")]
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
}
