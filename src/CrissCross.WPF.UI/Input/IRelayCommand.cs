// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace CrissCross.WPF.UI.Input;

/// <summary>
/// An interface expanding <see cref="ICommand"/> with the ability to raise
/// the <see cref="ICommand.CanExecuteChanged"/> event externally.
/// </summary>
public interface IRelayCommand : ICommand
{
    /// <summary>
    /// Notifies that the <see cref="ICommand.CanExecute"/> property has changed.
    /// </summary>
    void NotifyCanExecuteChanged();
}
