// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents an item in a <see cref="BreadcrumbBar"/> control.
/// </summary>
public class BreadcrumbBarItem : global::Avalonia.Controls.ContentControl
{
    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<object> IconProperty = AvaloniaProperty.Register<BreadcrumbBarItem, object>(
        nameof(Icon));

    /// <summary>
    /// Property for <see cref="IconMargin"/>.
    /// </summary>
    public static readonly StyledProperty<Thickness> IconMarginProperty = AvaloniaProperty.Register<BreadcrumbBarItem, Thickness>(
        nameof(IconMargin), new Thickness(0));

    /// <summary>
    /// Property for <see cref="IsLast"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsLastProperty = AvaloniaProperty.Register<BreadcrumbBarItem, bool>(
        nameof(IsLast), false);

    /// <summary>
    /// The self property.
    /// </summary>
    public static readonly StyledProperty<BreadcrumbBarItem> SelfProperty = AvaloniaProperty.Register<BreadcrumbBarItem, BreadcrumbBarItem>(
        nameof(Self));

    /// <summary>
    /// The navigation type property.
    /// </summary>
    public static readonly StyledProperty<Type> NavigationTypeProperty = AvaloniaProperty.Register<BreadcrumbBarItem, Type>(
        nameof(NavigationType));

    /// <summary>
    /// Initializes a new instance of the <see cref="BreadcrumbBarItem"/> class.
    /// </summary>
    public BreadcrumbBarItem() => Self = this;

    /// <summary>
    /// Gets or sets the type of the navigation.
    /// </summary>
    public Type NavigationType
    {
        get => GetValue(NavigationTypeProperty);
        set => SetValue(NavigationTypeProperty, value);
    }

    /// <summary>
    /// Gets the self.
    /// </summary>
    public BreadcrumbBarItem Self
    {
        get => GetValue(SelfProperty);
        private set => SetValue(SelfProperty, value);
    }

    /// <summary>
    /// Gets or sets displayed icon.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets margin for the icon.
    /// </summary>
    public Thickness IconMargin
    {
        get => GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the current item is the last one.
    /// </summary>
    public bool IsLast
    {
        get => GetValue(IsLastProperty);
        set => SetValue(IsLastProperty, value);
    }
}
