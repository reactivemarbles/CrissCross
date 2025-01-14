// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// PersonPictureTemplateSettings.
/// </summary>
/// <seealso cref="DependencyObject" />
public sealed class PersonPictureTemplateSettings : DependencyObject
{
    private static readonly DependencyPropertyKey ActualImageBrushPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ActualImageBrush),
            typeof(ImageBrush),
            typeof(PersonPictureTemplateSettings),
            null);

    private static readonly DependencyPropertyKey ActualInitialsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ActualInitials),
            typeof(string),
            typeof(PersonPictureTemplateSettings),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// The actual image brush property.
    /// </summary>
#pragma warning disable SA1202 // Elements should be ordered by access
    public static readonly DependencyProperty ActualImageBrushProperty =
        ActualImageBrushPropertyKey!.DependencyProperty;

    /// <summary>
    /// The actual initials property.
    /// </summary>
    public static readonly DependencyProperty ActualInitialsProperty =
        ActualInitialsPropertyKey!.DependencyProperty;
#pragma warning restore SA1202 // Elements should be ordered by access

    /// <summary>
    /// Gets the actual image brush.
    /// </summary>
    /// <value>
    /// The actual image brush.
    /// </value>
    public ImageBrush ActualImageBrush
    {
        get => (ImageBrush)GetValue(ActualImageBrushProperty);
        internal set => SetValue(ActualImageBrushPropertyKey, value);
    }

    /// <summary>
    /// Gets the actual initials.
    /// </summary>
    /// <value>
    /// The actual initials.
    /// </value>
    public string? ActualInitials
    {
        get => (string)GetValue(ActualInitialsProperty);
        internal set => SetValue(ActualInitialsPropertyKey, value);
    }
}
