// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays concise contextual status text with a semantic severity.</summary>
public class InfoBadge : Label
{
    /// <summary>Bindable property for <see cref="Severity"/>.</summary>
    public static readonly BindableProperty SeverityProperty = BindableProperty.Create(
        nameof(Severity),
        typeof(InfoBadgeSeverity),
        typeof(InfoBadge),
        InfoBadgeSeverity.Informational,
        propertyChanged: static (view, _, _) => ((InfoBadge)view).RefreshDescription());

    /// <summary>Initializes a new instance of the <see cref="InfoBadge"/> class.</summary>
    public InfoBadge() => RefreshDescription();

    /// <summary>Gets or sets the semantic severity used by the current theme.</summary>
    public InfoBadgeSeverity Severity
    {
        get => (InfoBadgeSeverity)GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    /// <summary>Updates the accessibility description from the semantic state.</summary>
    private void RefreshDescription() => SemanticProperties.SetDescription(this, $"{Severity}: {Text}");
}
