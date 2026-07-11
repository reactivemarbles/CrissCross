// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using Avalonia;
using AvaloniaInteractivity = global::Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>The modified password control. TextProperty contains asterisks OR raw password if IsPasswordRevealed is set to true, PasswordProperty always contains raw password.</summary>
public class PasswordBox : TextBox
{
    /// <summary>Property for <see cref="Password"/>.</summary>
    public static readonly StyledProperty<string> PasswordProperty = AvaloniaProperty.Register<PasswordBox, string>(
        nameof(Password),
        string.Empty);

    /// <summary>Property for <see cref="PasswordChar"/>.</summary>
    public static new readonly StyledProperty<char> PasswordCharProperty = AvaloniaProperty.Register<PasswordBox, char>(
        nameof(PasswordChar),
        '�');

    /// <summary>Property for <see cref="IsPasswordRevealed"/>.</summary>
    public static readonly StyledProperty<bool> IsPasswordRevealedProperty = AvaloniaProperty.Register<PasswordBox, bool>(
        nameof(IsPasswordRevealed),
        false);

    /// <summary>Property for <see cref="RevealButtonEnabled"/>.</summary>
    public static readonly StyledProperty<bool> RevealButtonEnabledProperty = AvaloniaProperty.Register<PasswordBox, bool>(
        nameof(RevealButtonEnabled),
        true);

    /// <summary>Property for <see cref="RevealPassword"/> (alias for IsPasswordRevealed for XAML compatibility).</summary>
    public static new readonly StyledProperty<bool> RevealPasswordProperty = AvaloniaProperty.Register<PasswordBox, bool>(
        nameof(RevealPassword),
        false);

    /// <summary>Event for "Password has changed".</summary>
    public static readonly AvaloniaInteractivity.RoutedEvent<AvaloniaInteractivity.RoutedEventArgs> PasswordChangedEvent =
        AvaloniaInteractivity.RoutedEvent.Register<PasswordBox, AvaloniaInteractivity.RoutedEventArgs>(
            nameof(PasswordChanged),
            AvaloniaInteractivity.RoutingStrategies.Bubble);

    /// <summary>Provides the _passwordHelper member.</summary>
    private readonly PasswordHelper _passwordHelper;

    /// <summary>Provides the _lockUpdatingContents member.</summary>
    private bool _lockUpdatingContents;

    /// <summary>Initializes a new instance of the <see cref="PasswordBox"/> class.</summary>
    public PasswordBox()
    {
        _lockUpdatingContents = false;
        _passwordHelper = new(this);
    }

    /// <summary>Event fired from this text box when its inner content has been changed.</summary>
    public event EventHandler<AvaloniaInteractivity.RoutedEventArgs>? PasswordChanged
    {
        add => AddHandler(PasswordChangedEvent, value);
        remove => RemoveHandler(PasswordChangedEvent, value);
    }

    /// <summary>Gets or sets currently typed text represented by asterisks.</summary>
    public string Password
    {
        get => GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    /// <summary>Gets or sets character used to mask the password.</summary>
    public new char PasswordChar
    {
        get => GetValue(PasswordCharProperty);
        set => SetValue(PasswordCharProperty, value);
    }

    /// <summary>Gets a value indicating whether the password is revealed.</summary>
    public bool IsPasswordRevealed
    {
        get => GetValue(IsPasswordRevealedProperty);
        private set => SetValue(IsPasswordRevealedProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether to reveal the password (XAML-friendly alias).</summary>
    public new bool RevealPassword
    {
        get => GetValue(RevealPasswordProperty);
        set => SetValue(RevealPasswordProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether to display the reveal password button.</summary>
    public bool RevealButtonEnabled
    {
        get => GetValue(RevealButtonEnabledProperty);
        set => SetValue(RevealButtonEnabledProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change is null)
        {
            return;
        }

        if (change.Property == PasswordProperty)
        {
            OnPasswordChanged();
        }
        else if (change.Property == PasswordCharProperty)
        {
            OnPasswordCharChanged();
        }
        else if (change.Property == IsPasswordRevealedProperty)
        {
            OnPasswordRevealModeChanged();
        }
        else if (change.Property == RevealPasswordProperty)
        {
            IsPasswordRevealed = RevealPassword;
        }
    }

    /// <inheritdoc />
    protected override void OnTextChanged()
    {
        UpdateTextContents(true);

        if (_lockUpdatingContents)
        {
            // Call base to handle basic text change
        }
        else
        {
            SetPlaceholderTextVisibility();
            RevealClearButton();
        }
    }

    /// <summary>Is called when <see cref="Password"/> property is changing.</summary>
    protected virtual void OnPasswordChanged() => UpdateTextContents(false);

    /// <summary>Is called when <see cref="PasswordChar"/> property is changing.</summary>
    protected virtual void OnPasswordCharChanged()
    {
        // If password is currently revealed, do not replace displayed text with asterisks
        if (IsPasswordRevealed)
        {
            return;
        }

        _lockUpdatingContents = true;
        Text = new(PasswordChar, Password.Length);
        _lockUpdatingContents = false;
    }

    /// <summary>Called when password reveal mode changed.</summary>
    protected virtual void OnPasswordRevealModeChanged()
    {
        _lockUpdatingContents = true;
        Text = IsPasswordRevealed ? Password : new string(PasswordChar, Password.Length);
        _lockUpdatingContents = false;
    }

    /// <summary>Triggered by clicking a button in the control template.</summary>
    /// <param name="parameter">Additional parameters.</param>
    protected override void OnTemplateButtonClick(string? parameter)
    {
        Debug.WriteLine($"INFO: {typeof(PasswordBox)} button clicked with param: {parameter}", "CrissCross.Avalonia.UI.PasswordBox");

        switch (parameter)
        {
            case "reveal":
                {
                    IsPasswordRevealed = !IsPasswordRevealed;
                    RevealPassword = IsPasswordRevealed;
                    _ = Focus();
                    CaretIndex = (Text ?? string.Empty).Length;
                    break;
                }

            default:
                {
                    base.OnTemplateButtonClick(parameter);
                    break;
                }
        }
    }

    /// <summary>Provides the UpdateTextContents member.</summary>
    /// <param name="isTriggeredByTextInput">The isTriggeredByTextInput value.</param>
    private void UpdateTextContents(bool isTriggeredByTextInput)
    {
        if (_lockUpdatingContents)
        {
            return;
        }

        if (IsPasswordRevealed)
        {
            UpdateRevealedTextContents(isTriggeredByTextInput);
            return;
        }

        UpdateMaskedTextContents(isTriggeredByTextInput);
    }

    /// <summary>Synchronizes text and password while the password is visible.</summary>
    /// <param name="isTriggeredByTextInput">Whether a text input change triggered the update.</param>
    private void UpdateRevealedTextContents(bool isTriggeredByTextInput)
    {
        if (Password == Text)
        {
            return;
        }

        _lockUpdatingContents = true;
        if (isTriggeredByTextInput)
        {
            Password = Text ?? string.Empty;
        }
        else
        {
            Text = Password;
            CaretIndex = (Text ?? string.Empty).Length;
        }

        RaiseEvent(new AvaloniaInteractivity.RoutedEventArgs(PasswordChangedEvent));
        _lockUpdatingContents = false;
    }

    /// <summary>Synchronizes masked text with the stored password.</summary>
    /// <param name="isTriggeredByTextInput">Whether a text input change triggered the update.</param>
    private void UpdateMaskedTextContents(bool isTriggeredByTextInput)
    {
        var caretIndex = CaretIndex;
        var newPasswordValue = _passwordHelper.GetPassword();

        if (isTriggeredByTextInput)
        {
            newPasswordValue = _passwordHelper.GetNewPassword();
        }

        _lockUpdatingContents = true;

        Text = new(PasswordChar, newPasswordValue?.Length ?? 0);
        Password = newPasswordValue ?? string.Empty;
        CaretIndex = caretIndex;

        RaiseEvent(new AvaloniaInteractivity.RoutedEventArgs(PasswordChangedEvent));
        _lockUpdatingContents = false;
    }

    /// <summary>Provides the PasswordHelper member.</summary>
    /// <param name="passwordBox">The passwordBox value.</param>
    private sealed class PasswordHelper(PasswordBox passwordBox)
    {
        /// <summary>Provides the _currentText member.</summary>
        private string _currentText = string.Empty;

        /// <summary>Provides the _newPasswordValue member.</summary>
        private string _newPasswordValue = string.Empty;

        /// <summary>Provides the _currentPassword member.</summary>
        private string _currentPassword = string.Empty;

        /// <summary>Provides the GetNewPassword member.</summary>
        /// <returns>The result.</returns>
        public string GetNewPassword()
        {
            _currentPassword = GetPassword();
            _newPasswordValue = _currentPassword;
            _currentText = passwordBox.Text ?? string.Empty;
            var selectionIndex = passwordBox.SelectionStart;
            var passwordChar = passwordBox.PasswordChar;
            var newCharacters = _currentText.Replace(passwordChar.ToString(), string.Empty);
            var isDeleted = false;

            if (IsDeleteOption())
            {
                _newPasswordValue = _currentPassword.Remove(
                    selectionIndex,
                    _currentPassword.Length - _currentText.Length);
                isDeleted = true;
            }

            switch (newCharacters.Length)
            {
                case > 1:
                    {
                        var index = _currentText.IndexOf(newCharacters[0]);

                        _newPasswordValue =
                            index > _newPasswordValue.Length - 1
                                ? _newPasswordValue + newCharacters
                                : _newPasswordValue.Insert(index, newCharacters);
                        break;
                    }

                case 1:
                    {
                        for (var i = 0; i < _currentText.Length; i++)
                        {
                            if (_currentText[i] == passwordChar)
                            {
                                continue;
                            }

                            UpdatePasswordWithInputCharacter(i, _currentText[i].ToString());
                            break;
                        }

                        break;
                    }

                case 0 when !isDeleted:
                    {
                        // The input is a PasswordChar, which is to be inserted at the designated position.
                        var insertIndex = selectionIndex - 1;
                        if (insertIndex >= 0)
                        {
                            UpdatePasswordWithInputCharacter(insertIndex, passwordChar.ToString());
                        }

                        break;
                    }
            }

            return _newPasswordValue;
        }

        /// <summary>Provides the GetPassword member.</summary>
        /// <returns>The result.</returns>
        public string GetPassword() => passwordBox.Password ?? string.Empty;

        /// <summary>Provides the UpdatePasswordWithInputCharacter member.</summary>
        /// <param name="insertIndex">The insertIndex value.</param>
        /// <param name="insertValue">The insertvalue.</param>
        private void UpdatePasswordWithInputCharacter(int insertIndex, string insertValue)
        {
            Debug.Assert(_currentText == (passwordBox.Text ?? string.Empty), "_currentText == passwordBox.Text");

            if (_currentText.Length == _newPasswordValue.Length)
            {
                // If it's a direct character replacement, remove the existing one before inserting the new one.
                _newPasswordValue = _newPasswordValue.Remove(insertIndex, 1).Insert(insertIndex, insertValue);
            }
            else
            {
                _newPasswordValue = _newPasswordValue.Insert(insertIndex, insertValue);
            }
        }

        /// <summary>Provides the IsDeleteOption member.</summary>
        /// <returns>The result.</returns>
        private bool IsDeleteOption()
        {
            Debug.Assert(_currentText == (passwordBox.Text ?? string.Empty), "_currentText == passwordBox.Text");
            Debug.Assert(_currentPassword == (passwordBox.Password ?? string.Empty), "_currentPassword == passwordBox.Password");

            return _currentText.Length < _currentPassword.Length;
        }
    }
}
