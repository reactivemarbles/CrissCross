// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// A <see cref="Page"/> that is reactive.
/// </summary>
/// <remarks>
/// <para>
/// This class is a <see cref="Page"/> that is also reactive. That is, it implements <see cref="IViewFor{TViewModel}"/>.
/// You can extend this class to get an implementation of <see cref="IViewFor{TViewModel}"/> rather than writing one yourself.
/// </para>
/// <para>
/// Note that the XAML for your control must specify the same base class, including the generic argument you provide for your view
/// model. To do this, use the <c>TypeArguments</c> attribute as follows:
/// <code>
/// <![CDATA[
/// <ui:ReactivePage
///         x:Class="Foo.Bar.Views.YourView"
///         x:TypeArguments="vms:YourViewModel"
///         xmlns:ui="https://github.com/ChrisPulman/CrissCross.ui"
///         xmlns:vms="clr-namespace:Foo.Bar.ViewModels"
///         xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
///         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
///         xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
///         xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
///         mc:Ignorable="d">
///     <!-- view XAML here -->
/// </ui:ReactivePage>
/// ]]>
/// </code>
/// </para>
/// </remarks>
/// <typeparam name="TViewModel">
/// The type of the view model backing the view.
/// </typeparam>
public class ReactivePage<TViewModel> :
    Page, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>
    /// The view model dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
                                    nameof(ViewModel),
                                    typeof(TViewModel),
                                    typeof(ReactivePage<TViewModel>),
                                    new PropertyMetadata(null));

    /// <summary>
    /// Gets the binding root view model.
    /// </summary>
    public TViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }
}
