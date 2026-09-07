// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays an empty, no-results, or error state with optional action commands.</summary>
public class EmptyState : ContentView
{
    /// <summary>Bindable property for <see cref="Model"/>.</summary>
    public static readonly BindableProperty ModelProperty = BindableProperty.Create(
        nameof(Model),
        typeof(EmptyStateModel),
        typeof(EmptyState),
        propertyChanged: static (view, _, _) => ((EmptyState)view).Refresh());

    /// <summary>Bindable property for <see cref="PrimaryCommand"/>.</summary>
    public static readonly BindableProperty PrimaryCommandProperty = BindableProperty.Create(
        nameof(PrimaryCommand),
        typeof(ICommand),
        typeof(EmptyState),
        propertyChanged: static (view, _, _) => ((EmptyState)view).Refresh());

    /// <summary>Provides the icon size used by the default native presentation.</summary>
    private const double IconFontSize = 32;

    /// <summary>Provides the title size used by the default native presentation.</summary>
    private const double TitleFontSize = 20;

    /// <summary>Provides default spacing between empty-state elements.</summary>
    private const double LayoutSpacing = 8;

    /// <summary>Provides default spacing between empty-state action buttons.</summary>
    private const double ActionSpacing = 8;

    /// <summary>Provides horizontal default content padding.</summary>
    private const double HorizontalPadding = 16;

    /// <summary>Provides vertical default content padding.</summary>
    private const double VerticalPadding = 24;

    /// <summary>Provides the accessibility text used when no model has been supplied.</summary>
    private const string FallbackTitle = "No content";

    /// <summary>Provides the primary text semantic resource key.</summary>
    private const string TextColorResourceKey = "CrissCrossTextColor";

    /// <summary>Provides the muted text semantic resource key.</summary>
    private const string MutedTextColorResourceKey = "CrissCrossMutedTextColor";

    /// <summary>Provides the accent fill semantic resource key.</summary>
    private const string AccentColorResourceKey = "CrissCrossAccentColor";

    /// <summary>Provides the accent text semantic resource key.</summary>
    private const string AccentTextColorResourceKey = "CrissCrossAccentTextColor";

    /// <summary>Provides the neutral surface semantic resource key.</summary>
    private const string NeutralSurfaceColorResourceKey = "CrissCrossNeutralSurfaceColor";

    /// <summary>Displays a semantic glyph for the current empty-state variant.</summary>
    private readonly Label _icon = new() { FontSize = IconFontSize, HorizontalTextAlignment = TextAlignment.Center };

    /// <summary>Displays the empty-state title.</summary>
    private readonly Label _title = new() { FontAttributes = FontAttributes.Bold, FontSize = TitleFontSize, HorizontalTextAlignment = TextAlignment.Center };

    /// <summary>Displays the empty-state message.</summary>
    private readonly Label _message = new() { HorizontalTextAlignment = TextAlignment.Center };

    /// <summary>Invokes the primary action when available.</summary>
    private readonly Button _primaryAction = new();

    /// <summary>Invokes the secondary action when the model provides one.</summary>
    private readonly Button _secondaryAction = new();

    /// <summary>Initializes a new instance of the <see cref="EmptyState"/> class.</summary>
    public EmptyState()
    {
        Content = CreateDefaultContent();
        Refresh();
    }

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public EmptyStateModel? Model
    {
        get => (EmptyStateModel?)GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    /// <summary>Gets or sets the command invoked by the control surface.</summary>
    public ICommand? PrimaryCommand
    {
        get => (ICommand?)GetValue(PrimaryCommandProperty);
        set => SetValue(PrimaryCommandProperty, value);
    }

    /// <summary>Gets the command currently projected by the primary button.</summary>
    private ICommand? EffectivePrimaryCommand => PrimaryCommand ?? Model?.PrimaryActionCommand;

    /// <summary>Combines title and message text for assistive technologies.</summary>
    /// <param name="title">The empty-state title.</param>
    /// <param name="message">The empty-state message.</param>
    /// <returns>The combined accessibility description.</returns>
    private static string GetAccessibleDescription(string title, string message) =>
        string.IsNullOrWhiteSpace(message) ? title : $"{title} {message}";

    /// <summary>Gets the glyph associated with an empty-state variant.</summary>
    /// <param name="variant">The empty-state variant.</param>
    /// <returns>The display glyph.</returns>
    private static string GetVariantGlyph(EmptyStateVariant variant) => variant switch
    {
        EmptyStateVariant.Error => "!",
        EmptyStateVariant.Offline => "↯",
        EmptyStateVariant.NoResults => "⌕",
        EmptyStateVariant.PermissionRequired => "⊘",
        _ => "∅",
    };

    /// <summary>Gets the title from the current model.</summary>
    /// <param name="model">The empty-state model.</param>
    /// <returns>The visible title.</returns>
    private static string GetTitle(EmptyStateModel? model)
    {
        var title = model?.Title;
        return string.IsNullOrWhiteSpace(title) ? FallbackTitle : title!;
    }

    /// <summary>Updates one action button from the current state snapshot.</summary>
    /// <param name="button">The button to update.</param>
    /// <param name="text">The action text.</param>
    /// <param name="command">The action command.</param>
    private static void RefreshAction(Button button, string? text, ICommand? command)
    {
        button.Text = text;
        button.Command = command;
        button.IsVisible = !string.IsNullOrWhiteSpace(text) && command is not null;
    }

    /// <summary>Creates the composed native default content.</summary>
    /// <returns>The default native content.</returns>
    private VerticalStackLayout CreateDefaultContent()
    {
        HorizontalStackLayout actions = new() { HorizontalOptions = LayoutOptions.Center, Spacing = ActionSpacing, Children = { _primaryAction, _secondaryAction } };

        _icon.SetDynamicResource(Label.TextColorProperty, MutedTextColorResourceKey);
        _title.SetDynamicResource(Label.TextColorProperty, TextColorResourceKey);
        _message.SetDynamicResource(Label.TextColorProperty, MutedTextColorResourceKey);
        _primaryAction.SetDynamicResource(Button.BackgroundColorProperty, AccentColorResourceKey);
        _primaryAction.SetDynamicResource(Button.TextColorProperty, AccentTextColorResourceKey);
        _secondaryAction.SetDynamicResource(Button.BackgroundColorProperty, NeutralSurfaceColorResourceKey);
        _secondaryAction.SetDynamicResource(Button.TextColorProperty, TextColorResourceKey);

        return new() { Padding = new(HorizontalPadding, VerticalPadding), Spacing = LayoutSpacing, HorizontalOptions = LayoutOptions.Center, Children = { _icon, _title, _message, actions } };
    }

    /// <summary>Updates the composed native content from the current model.</summary>
    private void Refresh()
    {
        var model = Model;
        var title = GetTitle(model);
        var message = model?.Message ?? string.Empty;
        _icon.Text = GetVariantGlyph(model?.Variant ?? EmptyStateVariant.NoData);
        _title.Text = title;
        _message.Text = message;
        _message.IsVisible = !string.IsNullOrWhiteSpace(message);
        RefreshAction(_primaryAction, model?.PrimaryActionText, EffectivePrimaryCommand);
        RefreshAction(_secondaryAction, model?.SecondaryActionText, model?.SecondaryActionCommand);
        SemanticProperties.SetDescription(this, GetAccessibleDescription(title, message));
    }
}
