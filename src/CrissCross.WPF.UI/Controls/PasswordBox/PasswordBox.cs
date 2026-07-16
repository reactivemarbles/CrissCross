// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

/// <summary>The modified password control. TextProperty contains asterisks OR raw password if IsPasswordRevealed is set
/// to true, PasswordProperty always contains raw password.</summary>
public partial class PasswordBox : TextBox
{
    /// <summary>Property for <see cref="Password"/>.</summary>
    public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
        nameof(Password),
        typeof(string),
        typeof(PasswordBox),
        new PropertyMetadata(string.Empty, static (d, _) => OnPasswordPropertyChanged((PasswordBox)d)));

    /// <summary>Property for <see cref="PasswordChar"/>.</summary>
    public static readonly DependencyProperty PasswordCharProperty = DependencyProperty.Register(
        nameof(PasswordChar),
        typeof(char),
        typeof(PasswordBox),
        new PropertyMetadata('*', static (d, _) => OnPasswordCharPropertyChanged((PasswordBox)d)));

    /// <summary>Property for <see cref="IsPasswordRevealed"/>.</summary>
    public static readonly DependencyProperty IsPasswordRevealedProperty = DependencyProperty.Register(
        nameof(IsPasswordRevealed),
        typeof(bool),
        typeof(PasswordBox),
        new PropertyMetadata(false, static (d, _) => OnPasswordRevealModePropertyChanged((PasswordBox)d)));

    /// <summary>Property for <see cref="RevealButtonEnabled"/>.</summary>
    public static readonly DependencyProperty RevealButtonEnabledProperty = DependencyProperty.Register(
        nameof(RevealButtonEnabled),
        typeof(bool),
        typeof(PasswordBox),
        new PropertyMetadata(true));

    /// <summary>Event for "Password has changed".</summary>
    public static readonly RoutedEvent PasswordChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(PasswordChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(PasswordBox));

    /// <summary>Stores the _passwordHelper value.</summary>
    private PasswordHelper _passwordHelper = null!;

    /// <summary>Stores the _lockUpdatingContents value.</summary>
    private bool _lockUpdatingContents;

    /// <summary>Initializes a new instance of the <see cref="PasswordBox"/> class.</summary>
    public PasswordBox()
    {
        _lockUpdatingContents = false;
    }

    /// <summary>Event fired from this text box when its inner content has been changed.</summary>
    /// <remarks>
    /// It is redirected from inner TextContainer.Changed event.
    /// </remarks>
    public event RoutedEventHandler PasswordChanged
    {
        add => AddHandler(PasswordChangedEvent, value);
        remove => RemoveHandler(PasswordChangedEvent, value);
    }

    /// <summary>Gets or sets currently typed text represented by asterisks.</summary>
    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    /// <summary>Gets or sets character used to mask the password.</summary>
    public char PasswordChar
    {
        get => (char)GetValue(PasswordCharProperty);
        set => SetValue(PasswordCharProperty, value);
    }

    /// <summary>Gets a value indicating whether the password is revealed.</summary>
    public bool IsPasswordRevealed
    {
        get => (bool)GetValue(IsPasswordRevealedProperty);
        private set => SetValue(IsPasswordRevealedProperty, value);
    }

    /// <summary>Gets or sets the GetValue value.</summary>
    public bool RevealButtonEnabled
    {
        get => (bool)GetValue(RevealButtonEnabledProperty);
        set => SetValue(RevealButtonEnabledProperty, value);
    }

    /// <inheritdoc />
    protected override void OnTextChanged(TextChangedEventArgs e)
    {
        UpdateTextContents(true);

        if (_lockUpdatingContents)
        {
            base.OnTextChanged(e);
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
        // If password is currently revealed,
        // do not replace displayed text with asterisks
        if (IsPasswordRevealed)
        {
            return;
        }

        _lockUpdatingContents = true;

        Text = new(PasswordChar, Password.Length);

        _lockUpdatingContents = false;
    }

    /// <summary>Called when [password reveal mode changed].</summary>
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
#if DEBUG
        System.Diagnostics.Debug.WriteLine(
            $"INFO: {typeof(PasswordBox)} button clicked with param: {parameter}",
            "CrissCross.WPF.UI.PasswordBox");
#endif

        switch (parameter)
        {
            case "reveal":
            {
                IsPasswordRevealed = !IsPasswordRevealed;
                _ = Focus();
                CaretIndex = Text.Length;
                break;
            }

            default:
            {
                base.OnTemplateButtonClick(parameter);
                break;
            }
        }
    }

    /// <summary>Called when <see cref="Password"/> is changed.</summary>
    /// <param name="control">The password box.</param>
    private static void OnPasswordPropertyChanged(PasswordBox control)
    {
        control.OnPasswordChanged();
    }

    /// <summary>Called if the character is changed in the during the run.</summary>
    /// <param name="control">The password box.</param>
    private static void OnPasswordCharPropertyChanged(PasswordBox control)
    {
        control.OnPasswordCharChanged();
    }

    /// <summary>Called if the reveal mode is changed in the during the run.</summary>
    /// <param name="control">The password box.</param>
    private static void OnPasswordRevealModePropertyChanged(PasswordBox control)
    {
        control.OnPasswordRevealModeChanged();
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
            if (Password == Text)
            {
                return;
            }

            _lockUpdatingContents = true;

            if (isTriggeredByTextInput)
            {
                Password = Text;
            }
            else
            {
                Text = Password;
                CaretIndex = Text.Length;
            }

            RaiseEvent(new RoutedEventArgs(PasswordChangedEvent));

            _lockUpdatingContents = false;

            return;
        }

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

        RaiseEvent(new RoutedEventArgs(PasswordChangedEvent));

        _lockUpdatingContents = false;
    }

    /// <summary>Provides the PasswordHelper member.</summary>
    /// <param name="passwordBox">The passwordBox value.</param>
    private sealed class PasswordHelper(PasswordBox passwordBox)
    {
        /// <summary>Stores the _currentText value.</summary>
        private string _currentText = string.Empty;

        /// <summary>Stores the _newPasswordvalue.</summary>
        private string _newPasswordValue = string.Empty;

        /// <summary>Stores the _currentPassword value.</summary>
        private string _currentPassword = string.Empty;

        /// <summary>Provides the GetNewPassword member.</summary>
        /// <returns>The result.</returns>
        public string GetNewPassword()
        {
            _currentPassword = GetPassword();
            _newPasswordValue = _currentPassword;
            _currentText = passwordBox.Text;
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
                    UpdatePasswordWithInputCharacter(insertIndex, passwordChar.ToString());
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
            Debug.Assert(_currentText == passwordBox.Text, "_currentText == _passwordBox.Text");

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
            Debug.Assert(_currentText == passwordBox.Text, "_currentText == _passwordBox.Text");
            Debug.Assert(_currentPassword == passwordBox.Password, "_currentPassword == _passwordBox.Password");

            return _currentText.Length < _currentPassword.Length;
        }
    }
}
