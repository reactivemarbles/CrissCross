// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia.Input;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Creates menu items for the rich text editor context menu.</summary>
internal static class RichTextMenuItemFactory
{
    /// <summary>Creates a context menu item with a control-key gesture.</summary>
    /// <param name="header">The item header.</param>
    /// <param name="key">The gesture key.</param>
    /// <param name="command">The command.</param>
    /// <returns>The configured menu item.</returns>
    public static global::Avalonia.Controls.MenuItem CreateGestureMenuItem(
        string header,
        Key key,
        ICommand command) =>
        new()
        {
            Header = header,
            InputGesture = new(key, KeyModifiers.Control),
            Command = command,
        };

    /// <summary>Creates a context menu item for a command.</summary>
    /// <param name="header">The item header.</param>
    /// <param name="command">The command.</param>
    /// <returns>The configured menu item.</returns>
    public static global::Avalonia.Controls.MenuItem CreateCommandMenuItem(string header, ICommand command) =>
        new() { Header = header, Command = command };

    /// <summary>Creates a submenu whose items invoke one command with different parameters.</summary>
    /// <param name="header">The submenu header.</param>
    /// <param name="command">The command.</param>
    /// <param name="choices">The item headers and command parameters.</param>
    /// <returns>The configured submenu.</returns>
    public static global::Avalonia.Controls.MenuItem CreateChoiceMenuItem(
        string header,
        ICommand command,
        (string Header, object Parameter)[] choices)
    {
        return new()
        {
            Header = header,
            ItemsSource = choices
                .Select(choice => new global::Avalonia.Controls.MenuItem
                {
                    Header = choice.Header,
                    Command = command,
                    CommandParameter = choice.Parameter,
                })
                .ToArray(),
        };
    }
}
