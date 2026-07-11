// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace CrissCross.WPF.UI;

/// <summary>MessageBox Show Mixins.</summary>
public static class MessageBoxShowMixins
{
    /// <summary>Provides the messageBoxFunc member.</summary>
    private static readonly Dictionary<string, SingleAssign<Func<Tuple<string, string, MessageBoxButton>, Task<MessageBoxResult>>>> messageBoxFunc = [];

    /// <summary>Provides the messageBoxCustomFunc member.</summary>
    private static readonly Dictionary<string, SingleAssign<Func<Tuple<string, string, string, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>, Task<CustomMessageBoxResult>>>> messageBoxCustomFunc = [];

    /// <summary>Provides the busyFunc member.</summary>
    private static readonly SingleAssign<Action<string, bool, string>> busyFunc = new();

    /// <summary>Provides extension members.</summary>
    /// <param name="requester">The requester value.</param>
    extension(ICanShowMessages? requester)
    {
        /// <summary>Displays a dismiss-able message-box. Click outside of the message area to dismiss.</summary>
        /// <exception cref="System.ArgumentNullException">owner.</exception>
        /// <exception cref="System.InvalidOperationException">No listener found for message box.</exception>
        /// <param name="bbcode">The text. Use BBCode to style the text.</param>
        /// <param name="title">The title.</param>
        /// <param name="messageBoxButton">The message box button.</param>
        /// <returns>
        /// Task of MessageBoxResult.
        /// </returns>
        public Task<MessageBoxResult> MessageBoxShow(string bbcode, string title = "", MessageBoxButton messageBoxButton = MessageBoxButton.OK)
        {
            var owner = GetValidatedOwner(requester);

            if (!messageBoxFunc.TryGetValue(owner, out var value))
            {
                throw new InvalidOperationException("No listener found for message box.");
            }

            return value.Value?.Invoke(new Tuple<string, string, MessageBoxButton>(bbcode, title, messageBoxButton)) ?? Task.FromResult(MessageBoxResult.None);
        }

        /// <summary>Displays a dismiss-able message-box. Click outside of the message area to dismiss, configure button text in custom buttons.</summary>
        /// <exception cref="System.ArgumentNullException">owner.</exception>
        /// <exception cref="System.InvalidOperationException">No listener found for message box.</exception>
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
        public Task<CustomMessageBoxResult> MessageBoxShow(string bbcode, string title, string custom0, string? custom1 = null, string? custom2 = null, string? custom3 = null, string? custom4 = null, string? custom5 = null, string? custom6 = null, string? custom7 = null, string? custom8 = null, string? custom9 = null)
        {
            var owner = GetValidatedOwner(requester);

            if (!messageBoxCustomFunc.TryGetValue(owner, out var value))
            {
                throw new InvalidOperationException("No listener found for message box.");
            }

            return value.Value?.Invoke(new Tuple<string, string, string, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>(bbcode, title, custom0, custom1, custom2, custom3, custom4, new Tuple<string?, string?, string?, string?, string?>(custom5, custom6, custom7, custom8, custom9))) ?? Task.FromResult(CustomMessageBoxResult.None);
        }

        /// <summary>Determines whether the specified call is busy.</summary>
        /// <param name="call">The call.</param>
        /// <param name="busy">if set to <c>true</c> [busy].</param>
        /// <param name="message">The message.</param>
        public void IsBusy(string call, bool busy, string message = "")
        {
            if (requester is null)
            {
                throw new ArgumentNullException(nameof(requester));
            }

            busyFunc.Value?.Invoke(call, busy, message);
        }
    }

    /// <summary>Provides extension members.</summary>
    /// <param name="dummy">The dummy value.</param>
    extension(IListenForMessages dummy)
    {
        /// <summary>Listens for busy.</summary>
        /// <param name="e">The e.</param>
        public void ListenForBusy(Action<string, bool, string> e)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            busyFunc.Assign(e);
        }

        /// <summary>Listens for messages.</summary>
        /// <param name="e">The e.</param>
        public void ListenForMessages(Func<Tuple<string, string, MessageBoxButton>, Task<MessageBoxResult>> e)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            if (dummy is not DependencyObject owner)
            {
                return;
            }

            var window = System.Windows.Window.GetWindow(owner);

            if (messageBoxFunc.ContainsKey(window.Name))
            {
                return;
            }

            SingleAssign<Func<Tuple<string, string, MessageBoxButton>, Task<MessageBoxResult>>> singleAssign = new();
            singleAssign.Assign(e);
            messageBoxFunc.Add(window.Name, singleAssign);
        }

        /// <summary>Listens for custom messages.</summary>
        /// <param name="e">The e.</param>
        public void ListenForCustomMessages(Func<Tuple<string, string, string, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>, Task<CustomMessageBoxResult>> e)
        {
            if (dummy is null)
            {
                throw new ArgumentNullException(nameof(dummy));
            }

            if (dummy is not DependencyObject owner)
            {
                return;
            }

            var window = System.Windows.Window.GetWindow(owner);

            if (messageBoxCustomFunc.ContainsKey(window.Name))
            {
                return;
            }

            SingleAssign<Func<Tuple<string, string, string, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>, Task<CustomMessageBoxResult>>> singleAssign = new();
            singleAssign.Assign(e);
            messageBoxCustomFunc.Add(window.Name, singleAssign);
        }
    }

    /// <summary>Gets the validated message requester owner.</summary>
    /// <param name="requester">The message requester.</param>
    /// <returns>The validated owner.</returns>
    private static string GetValidatedOwner(ICanShowMessages? requester)
    {
        if (requester is null)
        {
            throw new ArgumentNullException(nameof(requester));
        }

#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(requester.Owner);
        return requester.Owner;
#else
        if (string.IsNullOrWhiteSpace(requester.Owner))
        {
            throw new ArgumentException("The message requester owner cannot be null or whitespace.", nameof(requester));
        }

        return requester.Owner;
#endif
    }
}
