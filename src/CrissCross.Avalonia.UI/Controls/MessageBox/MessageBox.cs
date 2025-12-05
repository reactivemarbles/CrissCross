// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using ReactiveUI;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Customized window for notifications.
/// </summary>
public class MessageBox : Window
{
    /// <summary>
    /// Property for <see cref="ShowTitle"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ShowTitleProperty = AvaloniaProperty.Register<MessageBox, bool>(
        nameof(ShowTitle), true);

    /// <summary>
    /// Property for <see cref="PrimaryButtonText"/>.
    /// </summary>
    public static readonly StyledProperty<string> PrimaryButtonTextProperty = AvaloniaProperty.Register<MessageBox, string>(
        nameof(PrimaryButtonText), string.Empty);

    /// <summary>
    /// Property for <see cref="SecondaryButtonText"/>.
    /// </summary>
    public static readonly StyledProperty<string> SecondaryButtonTextProperty = AvaloniaProperty.Register<MessageBox, string>(
        nameof(SecondaryButtonText), string.Empty);

    /// <summary>
    /// Property for <see cref="CloseButtonText"/>.
    /// </summary>
    public static readonly StyledProperty<string> CloseButtonTextProperty = AvaloniaProperty.Register<MessageBox, string>(
        nameof(CloseButtonText), "Close");

    /// <summary>
    /// Property for <see cref="IsPrimaryButtonEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsPrimaryButtonEnabledProperty = AvaloniaProperty.Register<MessageBox, bool>(
        nameof(IsPrimaryButtonEnabled), true);

    /// <summary>
    /// Property for <see cref="IsSecondaryButtonEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsSecondaryButtonEnabledProperty = AvaloniaProperty.Register<MessageBox, bool>(
        nameof(IsSecondaryButtonEnabled), true);

    private TaskCompletionSource<MessageBoxResult>? _tcs;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBox"/> class.
    /// </summary>
    public MessageBox()
    {
        Topmost = true;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the Title.
    /// </summary>
    public bool ShowTitle
    {
        get => GetValue(ShowTitleProperty);
        set => SetValue(ShowTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to display on the primary button.
    /// </summary>
    public string PrimaryButtonText
    {
        get => GetValue(PrimaryButtonTextProperty);
        set => SetValue(PrimaryButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to be displayed on the secondary button.
    /// </summary>
    public string SecondaryButtonText
    {
        get => GetValue(SecondaryButtonTextProperty);
        set => SetValue(SecondaryButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to display on the close button.
    /// </summary>
    public string CloseButtonText
    {
        get => GetValue(CloseButtonTextProperty);
        set => SetValue(CloseButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the primary button is enabled.
    /// </summary>
    public bool IsPrimaryButtonEnabled
    {
        get => GetValue(IsPrimaryButtonEnabledProperty);
        set => SetValue(IsPrimaryButtonEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the secondary button is enabled.
    /// </summary>
    public bool IsSecondaryButtonEnabled
    {
        get => GetValue(IsSecondaryButtonEnabledProperty);
        set => SetValue(IsSecondaryButtonEnabledProperty, value);
    }

    /// <summary>
    /// Displays a message box.
    /// </summary>
    /// <param name="owner">The owner window.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>MessageBoxResult.</returns>
    public async Task<MessageBoxResult> ShowDialogAsync(Window? owner = null, CancellationToken cancellationToken = default)
    {
        _tcs = new TaskCompletionSource<MessageBoxResult>();

        var tokenRegistration = cancellationToken.Register(
            o => _tcs.TrySetCanceled((CancellationToken)o!),
            cancellationToken);

        try
        {
            if (owner != null)
            {
                await ShowDialog(owner);
            }
            else
            {
                Show();
            }

            return await _tcs.Task;
        }
        finally
        {
            await tokenRegistration.DisposeAsync();
        }
    }

    /// <summary>
    /// Closes the message box with the specified result.
    /// </summary>
    /// <param name="result">The result.</param>
    public void Close(MessageBoxResult result)
    {
        _tcs?.TrySetResult(result);
        Close();
    }

    /// <summary>
    /// Handles button click.
    /// </summary>
    /// <param name="button">The button.</param>
    protected virtual void OnButtonClick(MessageBoxButton button)
    {
        var result = button switch
        {
            MessageBoxButton.Primary => MessageBoxResult.Primary,
            MessageBoxButton.Secondary => MessageBoxResult.Secondary,
            _ => MessageBoxResult.None
        };

        Close(result);
    }
}
