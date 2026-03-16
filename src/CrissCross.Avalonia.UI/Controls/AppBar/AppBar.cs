// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A bottom application bar that can show and hide with animation, containing left and right content areas.
/// </summary>
public class AppBar : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="AppBarEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> AppBarEnabledProperty =
        AvaloniaProperty.Register<AppBar, bool>(nameof(AppBarEnabled), true);

    /// <summary>
    /// Property for <see cref="AppBarIsSticky"/>.
    /// </summary>
    public static readonly StyledProperty<bool> AppBarIsStickyProperty =
        AvaloniaProperty.Register<AppBar, bool>(nameof(AppBarIsSticky));

    /// <summary>
    /// Property for <see cref="AppBarLeft"/>.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<Control>> AppBarLeftProperty =
        AvaloniaProperty.Register<AppBar, ObservableCollection<Control>>(nameof(AppBarLeft));

    /// <summary>
    /// Property for <see cref="AppBarRight"/>.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<Control>> AppBarRightProperty =
        AvaloniaProperty.Register<AppBar, ObservableCollection<Control>>(nameof(AppBarRight));

    /// <summary>
    /// Property for <see cref="AppBarHeight"/>.
    /// </summary>
    public static readonly StyledProperty<double> AppBarHeightProperty =
        AvaloniaProperty.Register<AppBar, double>(nameof(AppBarHeight), 88.0);

    /// <summary>
    /// Property for <see cref="IsOpen"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<AppBar, bool>(nameof(IsOpen));

    private Border? _rootBorder;
    private bool _isPointerOver;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppBar"/> class.
    /// </summary>
    public AppBar()
    {
        AppBarLeft = [];
        AppBarRight = [];
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application bar is enabled.
    /// </summary>
    public bool AppBarEnabled
    {
        get => GetValue(AppBarEnabledProperty);
        set => SetValue(AppBarEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application bar stays open until explicitly closed.
    /// </summary>
    public bool AppBarIsSticky
    {
        get => GetValue(AppBarIsStickyProperty);
        set => SetValue(AppBarIsStickyProperty, value);
    }

    /// <summary>
    /// Gets or sets the left-aligned content items.
    /// </summary>
    public ObservableCollection<Control> AppBarLeft
    {
        get => GetValue(AppBarLeftProperty);
        set => SetValue(AppBarLeftProperty, value);
    }

    /// <summary>
    /// Gets or sets the right-aligned content items.
    /// </summary>
    public ObservableCollection<Control> AppBarRight
    {
        get => GetValue(AppBarRightProperty);
        set => SetValue(AppBarRightProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the application bar when expanded.
    /// </summary>
    public double AppBarHeight
    {
        get => GetValue(AppBarHeightProperty);
        set => SetValue(AppBarHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application bar is open.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Shows the application bar.
    /// </summary>
    /// <param name="isSticky">If set to <c>true</c>, the bar stays open until explicitly closed.</param>
    public void ShowAppBar(bool isSticky = false)
    {
        if (!AppBarEnabled)
        {
            HideAppBar();
            return;
        }

        AppBarIsSticky = isSticky;
        IsOpen = true;
    }

    /// <summary>
    /// Hides the application bar.
    /// </summary>
    public void HideAppBar() => IsOpen = false;

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnApplyTemplate(e);
        _rootBorder = e.NameScope.Find<Border>("PART_RootBorder");
    }

    /// <inheritdoc/>
    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        _isPointerOver = true;
    }

    /// <inheritdoc/>
    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        _isPointerOver = false;
    }

    /// <inheritdoc/>
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!_isPointerOver && IsOpen && !AppBarIsSticky)
        {
            HideAppBar();
        }
    }
}
