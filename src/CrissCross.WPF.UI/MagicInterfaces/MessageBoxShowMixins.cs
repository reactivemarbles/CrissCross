// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>MessageBox Show Mixins.</summary>
public static class MessageBoxShowMixins
{
    /// <summary>Provides the messageBoxFunc member.</summary>
    private static readonly Dictionary<
        string,
        SingleAssign<Func<Tuple<string, string, MessageBoxButton>, Task<MessageBoxResult>>>
    > messageBoxFunc = [];

    /// <summary>Provides the messageBoxCustomFunc member.</summary>
    private static readonly Dictionary<
        string,
        SingleAssign<Func<CustomMessageBoxRequest, Task<CustomMessageBoxResult>>>
    > messageBoxCustomFunc = [];

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
        /// <returns>Task of MessageBoxResult.</returns>
        public Task<MessageBoxResult> MessageBoxShow(string bbcode) =>
            requester.MessageBoxShow(bbcode, string.Empty, MessageBoxButton.OK);

        /// <summary>Displays a dismiss-able message-box.</summary>
        /// <param name="bbcode">The text. Use BBCode to style the text.</param>
        /// <param name="title">The title.</param>
        /// <returns>Task of MessageBoxResult.</returns>
        public Task<MessageBoxResult> MessageBoxShow(string bbcode, string title) =>
            requester.MessageBoxShow(bbcode, title, MessageBoxButton.OK);

        /// <summary>Displays a dismiss-able message-box.</summary>
        /// <param name="bbcode">The text. Use BBCode to style the text.</param>
        /// <param name="title">The title.</param>
        /// <param name="messageBoxButton">The message box button.</param>
        /// <returns>Task of MessageBoxResult.</returns>
        public Task<MessageBoxResult> MessageBoxShow(string bbcode, string title, MessageBoxButton messageBoxButton)
        {
            var owner = GetValidatedOwner(requester);

            if (!messageBoxFunc.TryGetValue(owner, out var value))
            {
                throw new InvalidOperationException("No listener found for message box.");
            }

            return value.Value?.Invoke(new Tuple<string, string, MessageBoxButton>(bbcode, title, messageBoxButton))
                ?? Task.FromResult(MessageBoxResult.None);
        }

        /// <summary>Displays a dismiss-able message-box. Click outside of the message area to dismiss, configure button
        /// text in custom buttons.</summary>
        /// <exception cref="System.ArgumentNullException">owner.</exception>
        /// <exception cref="System.InvalidOperationException">No listener found for message box.</exception>
        /// <param name="request">The custom message request.</param>
        /// <returns>A Value.</returns>
        public Task<CustomMessageBoxResult> MessageBoxShow(CustomMessageBoxRequest request)
        {
            var owner = GetValidatedOwner(requester);

            if (!messageBoxCustomFunc.TryGetValue(owner, out var value))
            {
                throw new InvalidOperationException("No listener found for message box.");
            }

            return value.Value?.Invoke(request) ?? Task.FromResult(CustomMessageBoxResult.None);
        }

        /// <summary>Determines whether the specified call is busy.</summary>
        /// <param name="call">The call.</param>
        /// <param name="busy">if set to <c>true</c> [busy].</param>
        public void IsBusy(string call, bool busy) => requester.IsBusy(call, busy, string.Empty);

        /// <summary>Determines whether the specified call is busy.</summary>
        /// <param name="call">The call.</param>
        /// <param name="busy">if set to <c>true</c> [busy].</param>
        /// <param name="message">The message.</param>
        public void IsBusy(string call, bool busy, string message)
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
        public void ListenForCustomMessages(Func<CustomMessageBoxRequest, Task<CustomMessageBoxResult>> e)
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

            SingleAssign<Func<CustomMessageBoxRequest, Task<CustomMessageBoxResult>>> singleAssign = new();
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
