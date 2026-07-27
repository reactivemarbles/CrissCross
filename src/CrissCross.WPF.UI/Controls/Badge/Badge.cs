// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Used to highlight an item, attract attention or flag status.</summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:Badge Appearance="Secondary"&gt;
///     &lt;TextBox Text="Hello" /&gt;
/// &lt;/ui:Badge&gt;
/// </code>
/// </example>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Badge : System.Windows.Controls.ContentControl, IAppearanceControl
{
    /// <summary>Property for <see cref="Appearance"/>.</summary>
    public static readonly DependencyProperty AppearanceProperty = DependencyProperty.Register(
        nameof(Appearance),
        typeof(ControlAppearance),
        typeof(Badge),
        new(ControlAppearance.Primary));

    /// <inheritdoc />
    public ControlAppearance Appearance
    {
        get => (ControlAppearance)GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
