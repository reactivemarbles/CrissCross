// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace CrissCross.WPF.UI;

#pragma warning disable RCS1175 // Unused 'this' parameter
/// <summary>
/// MessageBox Show Mixins.
/// </summary>
public static class MessageBoxShowMixins
{
    private static readonly SingleAssign<Func<Tuple<string, string, MessageBoxButton>, Task<MessageBoxResult>>> messageBoxFunc = new();
    private static readonly SingleAssign<Func<Tuple<string, string, string, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>, Task<CustomMessageBoxResult>>> messageBoxCustomFunc = new();
    private static readonly SingleAssign<Action<string, bool, string>> busyFunc = new();

    /// <summary>
    /// Listens for busy.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="e">The e.</param>
    public static void ListenForBusy(this IListenForMessages dummy, Action<string, bool, string> e) => busyFunc.Assign(e);

    /// <summary>
    /// Listens for messages.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="e">The e.</param>
    public static void ListenForMessages(this IListenForMessages dummy, Func<Tuple<string, string, MessageBoxButton>, Task<MessageBoxResult>> e) =>
        messageBoxFunc.Assign(e);

    /// <summary>
    /// Listens for custom messages.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="e">The e.</param>
    public static void ListenForCustomMessages(this IListenForMessages dummy, Func<Tuple<string, string, string, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>, Task<CustomMessageBoxResult>> e) =>
        messageBoxCustomFunc.Assign(e);

    /// <summary>
    /// Displays a dismiss-able message-box. Click outside of the message area to dismiss.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="bbcode">The text. Use BBCode to style the text.</param>
    /// <param name="title">The title.</param>
    /// <param name="messageBoxButton">The message box button.</param>
    /// <returns>
    /// Task of MessageBoxResult.
    /// </returns>
    public static Task<MessageBoxResult> MessageBoxShow(this ICanShowMessages? dummy, string bbcode, string title = "", MessageBoxButton messageBoxButton = MessageBoxButton.OK) =>
        messageBoxFunc.Value?.Invoke(new Tuple<string, string, MessageBoxButton>(bbcode, title, messageBoxButton)) ?? Task.FromResult(MessageBoxResult.None);

    /// <summary>
    /// Displays a dismiss-able message-box. Click outside of the message area to dismiss, configure button text in custom buttons.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="bbcode">The bbcode.</param>
    /// <param name="title">The title.</param>
    /// <param name="custom0">The custom0 button text.</param>
    /// <param name="custom1">The custom1 button text.</param>
    /// <param name="custom2">The custom2 button text.</param>
    /// <param name="custom3">The custom3 button text.</param>
    /// <param name="custom4">The custom4 button text.</param>
    /// <param name="custom5">The custom5 button text.</param>
    /// <param name="custom6">The custom6 button text.</param>
    /// <param name="custom7">The custom7 button text.</param>
    /// <param name="custom8">The custom8 button text.</param>
    /// <param name="custom9">The custom9 button text.</param>
    /// <returns>
    /// A Value.
    /// </returns>
    public static Task<CustomMessageBoxResult> MessageBoxShow(this ICanShowMessages? dummy, string bbcode, string title, string custom0, string? custom1 = null, string? custom2 = null, string? custom3 = null, string? custom4 = null, string? custom5 = null, string? custom6 = null, string? custom7 = null, string? custom8 = null, string? custom9 = null) =>
        messageBoxCustomFunc.Value?.Invoke(new Tuple<string, string, string, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>(bbcode, title, custom0, custom1, custom2, custom3, custom4, new Tuple<string?, string?, string?, string?, string?>(custom5, custom6, custom7, custom8, custom9))) ?? Task.FromResult(CustomMessageBoxResult.None);

    /// <summary>
    /// Determines whether the specified call is busy.
    /// </summary>
    /// <param name="dummy">The dummy.</param>
    /// <param name="call">The call.</param>
    /// <param name="busy">if set to <c>true</c> [busy].</param>
    /// <param name="message">The message.</param>
    public static void IsBusy(this ICanShowMessages dummy, string call, bool busy, string message = "") =>
        busyFunc.Value?.Invoke(call, busy, message);
}
#pragma warning restore RCS1175 // Unused 'this' parameter

