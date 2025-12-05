// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// MessageBoxAsync provides static methods to show message boxes asynchronously.
/// </summary>
public static class MessageBoxAsync
{
    /// <summary>
    /// Shows a message box with the specified text.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="title">The title.</param>
    /// <param name="buttons">The buttons to show.</param>
    /// <param name="owner">The owner window.</param>
    /// <returns>MessageBoxResult.</returns>
    public static Task<MessageBoxResult> ShowAsync(
        string text,
        string title = "",
        MessageBoxButton buttons = MessageBoxButton.Close,
        Window? owner = null)
    {
        var messageBox = new MessageBox
        {
            Title = title,
            Content = text
        };

        ConfigureButtons(messageBox, buttons);

        return messageBox.ShowDialogAsync(owner);
    }

    private static void ConfigureButtons(MessageBox messageBox, MessageBoxButton buttons)
    {
        switch (buttons)
        {
            case MessageBoxButton.Primary:
                messageBox.PrimaryButtonText = "OK";
                messageBox.SecondaryButtonText = string.Empty;
                messageBox.CloseButtonText = string.Empty;
                break;
            case MessageBoxButton.Secondary:
                messageBox.PrimaryButtonText = string.Empty;
                messageBox.SecondaryButtonText = "Cancel";
                messageBox.CloseButtonText = string.Empty;
                break;
            case MessageBoxButton.Close:
                messageBox.PrimaryButtonText = string.Empty;
                messageBox.SecondaryButtonText = string.Empty;
                messageBox.CloseButtonText = "Close";
                break;
        }
    }
}
