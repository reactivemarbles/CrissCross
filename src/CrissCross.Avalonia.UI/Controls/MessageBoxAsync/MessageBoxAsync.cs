// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>MessageBoxAsync provides static methods to show message boxes asynchronously.</summary>
public static class MessageBoxAsync
{
    /// <summary>Displays a message box.</summary>
    /// <param name="text">The text to display.</param>
    /// <returns>MessageBoxResult.</returns>
    public static Task<MessageBoxResult> ShowAsync(string text) =>
        ShowAsync(text, string.Empty, MessageBoxButton.Close, null);

    /// <summary>Displays a message box.</summary>
    /// <param name="text">The text to display.</param>
    /// <param name="title">The title.</param>
    /// <returns>MessageBoxResult.</returns>
    public static Task<MessageBoxResult> ShowAsync(string text, string title) =>
        ShowAsync(text, title, MessageBoxButton.Close, null);

    /// <summary>Displays a message box.</summary>
    /// <param name="text">The text to display.</param>
    /// <param name="title">The title.</param>
    /// <param name="buttons">The buttons to show.</param>
    /// <returns>MessageBoxResult.</returns>
    public static Task<MessageBoxResult> ShowAsync(string text, string title, MessageBoxButton buttons) =>
        ShowAsync(text, title, buttons, null);

    /// <summary>Shows a message box with the specified text.</summary>
    /// <param name="text">The text to display.</param>
    /// <param name="title">The title.</param>
    /// <param name="buttons">The buttons to show.</param>
    /// <param name="owner">The owner window.</param>
    /// <returns>MessageBoxResult.</returns>
    public static Task<MessageBoxResult> ShowAsync(
        string text,
        string title,
        MessageBoxButton buttons,
        Window? owner)
    {
        var messageBox = new MessageBox
        {
            Title = title,
            Content = text
        };

        ConfigureButtons(messageBox, buttons);

        return messageBox.ShowDialogAsync(owner);
    }

    /// <summary>Provides the ConfigureButtons member.</summary>
    /// <param name="messageBox">The messageBox value.</param>
    /// <param name="buttons">The buttons value.</param>
    private static void ConfigureButtons(MessageBox messageBox, MessageBoxButton buttons)
    {
        switch (buttons)
        {
            case MessageBoxButton.Primary:
                {
                    messageBox.PrimaryButtonText = "OK";
                    messageBox.SecondaryButtonText = string.Empty;
                    messageBox.CloseButtonText = string.Empty;
                    break;
                }

            case MessageBoxButton.Secondary:
                {
                    messageBox.PrimaryButtonText = string.Empty;
                    messageBox.SecondaryButtonText = "Cancel";
                    messageBox.CloseButtonText = string.Empty;
                    break;
                }

            case MessageBoxButton.Close:
                {
                    messageBox.PrimaryButtonText = string.Empty;
                    messageBox.SecondaryButtonText = string.Empty;
                    messageBox.CloseButtonText = "Close";
                    break;
                }
        }
    }
}
