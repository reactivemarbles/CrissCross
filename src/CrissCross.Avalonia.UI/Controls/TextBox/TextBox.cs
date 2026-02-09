// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a text input control that supports placeholder text, an optional icon, and a clear button, with
/// additional customization options for user interaction.
/// </summary>
/// <remarks>The TextBox control extends the standard Avalonia TextBox by providing features such as configurable
/// placeholder text, an optional icon with customizable placement, and a clear button that can be enabled or disabled.
/// The clear button appears when the control is focused and contains text, allowing users to quickly clear the input.
/// The control also exposes commands for template button interactions and supports enabling or disabling text
/// selection. These features make it suitable for scenarios where enhanced user experience and input customization are
/// required.</remarks>
public class TextBox : global::Avalonia.Controls.TextBox
{
    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<TextBox, object?>(
        nameof(Icon));

    /// <summary>
    /// Property for <see cref="IconPlacement"/>.
    /// </summary>
    public static readonly StyledProperty<ElementPlacement> IconPlacementProperty = AvaloniaProperty.Register<TextBox, ElementPlacement>(
        nameof(IconPlacement), ElementPlacement.Left);

    /// <summary>
    /// Property for <see cref="PlaceholderText"/>.
    /// </summary>
    public static readonly StyledProperty<string> PlaceholderTextProperty = AvaloniaProperty.Register<TextBox, string>(
        nameof(PlaceholderText), string.Empty);

    /// <summary>
    /// Property for <see cref="PlaceholderEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> PlaceholderEnabledProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(PlaceholderEnabled), true);

    /// <summary>
    /// Property for <see cref="CurrentPlaceholderEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> CurrentPlaceholderEnabledProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(CurrentPlaceholderEnabled), true);

    /// <summary>
    /// Property for <see cref="ClearButtonEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ClearButtonEnabledProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(ClearButtonEnabled), true);

    /// <summary>
    /// Property for <see cref="ShowClearButton"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ShowClearButtonProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(ShowClearButton), false);

    /// <summary>
    /// Property for <see cref="IsTextSelectionEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsTextSelectionEnabledProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(IsTextSelectionEnabled), false);

    /// <summary>
    /// Property for <see cref="TemplateButtonCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> TemplateButtonCommandProperty = AvaloniaProperty.Register<TextBox, ICommand?>(
        nameof(TemplateButtonCommand));

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBox"/> class.
    /// </summary>
    public TextBox()
    {
        TemplateButtonCommand = ReactiveCommand.Create<string?>(OnTemplateButtonClick);
        CurrentPlaceholderEnabled = PlaceholderEnabled;
    }

    /// <summary>
    /// Gets or sets displayed icon.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets which side the icon should be placed on.
    /// </summary>
    public ElementPlacement IconPlacement
    {
        get => GetValue(IconPlacementProperty);
        set => SetValue(IconPlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets placeholder text.
    /// </summary>
    public string PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to enable the placeholder text.
    /// </summary>
    public bool PlaceholderEnabled
    {
        get => GetValue(PlaceholderEnabledProperty);
        set => SetValue(PlaceholderEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to display the placeholder text.
    /// </summary>
    public bool CurrentPlaceholderEnabled
    {
        get => GetValue(CurrentPlaceholderEnabledProperty);
        protected set => SetValue(CurrentPlaceholderEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to enable the clear button.
    /// </summary>
    public bool ClearButtonEnabled
    {
        get => GetValue(ClearButtonEnabledProperty);
        set => SetValue(ClearButtonEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the clear button when <see cref="TextBox"/> is focused.
    /// </summary>
    public bool ShowClearButton
    {
        get => GetValue(ShowClearButtonProperty);
        protected set => SetValue(ShowClearButtonProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether text selection is enabled.
    /// </summary>
    public bool IsTextSelectionEnabled
    {
        get => GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }

    /// <summary>
    /// Gets the command triggered when clicking the button.
    /// </summary>
    public ICommand? TemplateButtonCommand
    {
        get => GetValue(TemplateButtonCommandProperty);
        private set => SetValue(TemplateButtonCommandProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change is null)
        {
            return;
        }

        if (change.Property == TextProperty)
        {
            OnTextChanged();
        }
        else if (change.Property == PlaceholderEnabledProperty)
        {
            OnPlaceholderEnabledChanged();
        }
    }

    /// <summary>
    /// Called when text changes.
    /// </summary>
    protected virtual void OnTextChanged()
    {
        SetPlaceholderTextVisibility();
        RevealClearButton();
    }

    /// <summary>
    /// Sets the placeholder text visibility.
    /// </summary>
    protected void SetPlaceholderTextVisibility()
    {
        var text = Text ?? string.Empty;

        if (PlaceholderEnabled)
        {
            if (CurrentPlaceholderEnabled && text.Length > 0)
            {
                SetValue(CurrentPlaceholderEnabledProperty, false);
            }

            if (!CurrentPlaceholderEnabled && text.Length < 1)
            {
                SetValue(CurrentPlaceholderEnabledProperty, true);
            }
        }
        else if (CurrentPlaceholderEnabled)
        {
            SetValue(CurrentPlaceholderEnabledProperty, false);
        }
    }

    /// <inheritdoc />
    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);

        CaretIndex = (Text ?? string.Empty).Length;
        RevealClearButton();
    }

    /// <inheritdoc />
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        HideClearButton();
    }

    /// <summary>
    /// Reveals the clear button by <see cref="ShowClearButton"/> property.
    /// </summary>
    protected void RevealClearButton()
    {
        if (ClearButtonEnabled && IsFocused)
        {
            SetValue(ShowClearButtonProperty, (Text ?? string.Empty).Length > 0);
        }
    }

    /// <summary>
    /// Hides the clear button by <see cref="ShowClearButton"/> property.
    /// </summary>
    protected void HideClearButton()
    {
        if (ClearButtonEnabled && !IsFocused && ShowClearButton)
        {
            SetValue(ShowClearButtonProperty, false);
        }
    }

    /// <summary>
    /// Triggered when the user clicks the clear text button.
    /// </summary>
    protected virtual void OnClearButtonClick()
    {
        if ((Text ?? string.Empty).Length > 0)
        {
            Text = string.Empty;
        }
    }

    /// <summary>
    /// Triggered by clicking a button in the control template.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    protected virtual void OnTemplateButtonClick(string? parameter)
    {
        Debug.WriteLine($"INFO: {typeof(TextBox)} button clicked", "CrissCross.Avalonia.UI.TextBox");
        OnClearButtonClick();
    }

    /// <summary>
    /// Called when placeholder enabled property changes.
    /// </summary>
    protected virtual void OnPlaceholderEnabledChanged() => SetPlaceholderTextVisibility();
}
