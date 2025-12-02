// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// The modified password control.
/// </summary>
public class PasswordBox : TextBox
{
    /// <summary>
    /// Property for <see cref="PasswordChar"/>.
    /// </summary>
    public static readonly StyledProperty<char> PasswordCharProperty = AvaloniaProperty.Register<PasswordBox, char>(
        nameof(PasswordChar), '*');

    /// <summary>
    /// Gets or sets character used to mask the password.
    /// </summary>
    public char PasswordChar
    {
        get => GetValue(PasswordCharProperty);
        set => SetValue(PasswordCharProperty, value);
    }
}
