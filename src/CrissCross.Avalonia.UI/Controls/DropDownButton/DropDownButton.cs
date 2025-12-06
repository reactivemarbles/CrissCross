// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a button with a drop-down list.
/// </summary>
public class DropDownButton : Button
{
    /// <summary>
    /// Property for <see cref="Flyout"/>.
    /// </summary>
    public static readonly StyledProperty<FlyoutBase?> FlyoutProperty =
        AvaloniaProperty.Register<DropDownButton, FlyoutBase?>(nameof(Flyout));

    /// <summary>
    /// Property for <see cref="IsDropDownOpen"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<DropDownButton, bool>(nameof(IsDropDownOpen));

    /// <summary>
    /// Gets or sets the flyout associated with this button.
    /// </summary>
    public new FlyoutBase? Flyout
    {
        get => GetValue(FlyoutProperty);
        set => SetValue(FlyoutProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dropdown is open.
    /// </summary>
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnClick()
    {
        base.OnClick();

        if (Flyout != null)
        {
            Flyout.ShowAt(this);
            SetCurrentValue(IsDropDownOpenProperty, true);
        }
    }
}
