// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays grouped validation messages for a form or editor surface.</summary>
public class ValidationSummary : ContentView
{
    /// <summary>Bindable property for <see cref="SummaryState"/>.</summary>
    public static readonly BindableProperty SummaryStateProperty = BindableProperty.Create(
        nameof(SummaryState),
        typeof(ValidationSummaryState),
        typeof(ValidationSummary),
        propertyChanged: static (view, _, _) => ((ValidationSummary)view).Refresh());

    /// <summary>Provides the heading label font size.</summary>
    private const double HeadingFontSize = 16;

    /// <summary>Provides spacing between summary rows.</summary>
    private const double LayoutSpacing = 6;

    /// <summary>Provides default control padding.</summary>
    private const double ControlPadding = 12;

    /// <summary>Provides indentation for validation message rows.</summary>
    private const double MessageIndent = 12;

    /// <summary>Provides the primary text semantic resource key.</summary>
    private const string TextColorResourceKey = "CrissCrossTextColor";

    /// <summary>Provides the muted text semantic resource key.</summary>
    private const string MutedTextColorResourceKey = "CrissCrossMutedTextColor";

    /// <summary>Provides the danger semantic resource key.</summary>
    private const string DangerColorResourceKey = "CrissCrossDangerColor";

    /// <summary>Provides the caution semantic resource key.</summary>
    private const string CautionColorResourceKey = "CrissCrossCautionColor";

    /// <summary>Provides the success semantic resource key.</summary>
    private const string SuccessColorResourceKey = "CrissCrossSuccessColor";

    /// <summary>Displays the validation summary heading.</summary>
    private readonly Label _heading = new() { FontAttributes = FontAttributes.Bold, FontSize = HeadingFontSize };

    /// <summary>Displays validation message rows.</summary>
    private readonly VerticalStackLayout _messages = new() { Spacing = LayoutSpacing };

    /// <summary>Initializes a new instance of the <see cref="ValidationSummary"/> class.</summary>
    public ValidationSummary()
    {
        _heading.SetDynamicResource(Label.TextColorProperty, TextColorResourceKey);
        Content = new VerticalStackLayout { Padding = new(ControlPadding), Spacing = LayoutSpacing, Children = { _heading, _messages } };
        Refresh();
    }

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public ValidationSummaryState? SummaryState
    {
        get => (ValidationSummaryState?)GetValue(SummaryStateProperty);
        set => SetValue(SummaryStateProperty, value);
    }

    /// <summary>Gets a text prefix for a validation severity.</summary>
    /// <param name="severity">The validation severity.</param>
    /// <returns>The visible severity prefix.</returns>
    private static string GetSeverityPrefix(ValidationSeverity severity) => severity switch
    {
        ValidationSeverity.Error => "Error:",
        ValidationSeverity.Warning => "Warning:",
        ValidationSeverity.Pending => "Pending:",
        ValidationSeverity.Success => "Success:",
        _ => "Info:",
    };

    /// <summary>Gets a semantic text resource for a validation severity.</summary>
    /// <param name="severity">The validation severity.</param>
    /// <returns>The dynamic resource key.</returns>
    private static string GetSeverityTextResource(ValidationSeverity severity) => severity switch
    {
        ValidationSeverity.Error => DangerColorResourceKey,
        ValidationSeverity.Warning => CautionColorResourceKey,
        ValidationSeverity.Pending => MutedTextColorResourceKey,
        ValidationSeverity.Success => SuccessColorResourceKey,
        _ => TextColorResourceKey,
    };

    /// <summary>Creates a label for one validation message.</summary>
    /// <param name="message">The validation message.</param>
    /// <returns>The configured label.</returns>
    private static Label CreateMessageLabel(ValidationMessage message)
    {
        Label label = new() { Margin = new(MessageIndent, 0, 0, 0), Text = $"{GetSeverityPrefix(message.Severity)} {message.DisplayText}" };
        label.SetDynamicResource(Label.TextColorProperty, GetSeverityTextResource(message.Severity));
        return label;
    }

    /// <summary>Updates the composed native content from the current validation snapshot.</summary>
    private void Refresh()
    {
        var state = SummaryState;
        var summaryText = state?.SummaryText ?? "No validation messages";
        _heading.Text = summaryText;
        _messages.Children.Clear();

        if (state is not null)
        {
            foreach (var message in state.Messages)
            {
                _messages.Children.Add(CreateMessageLabel(message));
            }
        }

        _messages.IsVisible = _messages.Children.Count > 0;
        SemanticProperties.SetDescription(this, summaryText);
    }
}
