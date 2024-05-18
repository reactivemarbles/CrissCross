// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a rich editing control which operates on System.Windows.Documents.FlowDocument objects.
/// </summary>
public class RichTextBox : System.Windows.Controls.RichTextBox
{
    /// <summary>
    /// Property for <see cref="IsTextSelectionEnabledProperty"/>.
    /// </summary>
    public static readonly DependencyProperty IsTextSelectionEnabledProperty = DependencyProperty.Register(
        nameof(IsTextSelectionEnabled),
        typeof(bool),
        typeof(RichTextBox),
        new PropertyMetadata(false));

    /// <summary>
    /// Gets or sets a value indicating whether this instance is text selection enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is text selection enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsTextSelectionEnabled
    {
        get => (bool)GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }
}
