// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents PersonPictureTemplateSettings.</summary>
/// <seealso cref="DependencyObject" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class PersonPictureTemplateSettings : DependencyObject
{
    /// <summary>Provides the ActualImageBrushPropertyKey member.</summary>
    public static readonly DependencyPropertyKey ActualImageBrushPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ActualImageBrush),
        typeof(ImageBrush),
        typeof(PersonPictureTemplateSettings),
        null);

    /// <summary>Provides the ActualInitialsPropertyKey member.</summary>
    public static readonly DependencyPropertyKey ActualInitialsPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ActualInitials),
        typeof(string),
        typeof(PersonPictureTemplateSettings),
        new(string.Empty));

    /// <summary>The actual image brush property.</summary>
    public static readonly DependencyProperty ActualImageBrushProperty = ActualImageBrushPropertyKey.DependencyProperty;

    /// <summary>The actual initials property.</summary>
    public static readonly DependencyProperty ActualInitialsProperty = ActualInitialsPropertyKey.DependencyProperty;

    /// <summary>Gets the actual image brush.</summary>
    /// <value>
    /// The actual image brush.
    /// </value>
    public ImageBrush ActualImageBrush
    {
        get => (ImageBrush)GetValue(ActualImageBrushProperty);
        internal set => SetValue(ActualImageBrushPropertyKey, value);
    }

    /// <summary>Gets the actual initials.</summary>
    /// <value>
    /// The actual initials.
    /// </value>
    public string? ActualInitials
    {
        get => (string)GetValue(ActualInitialsProperty);
        internal set => SetValue(ActualInitialsPropertyKey, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
