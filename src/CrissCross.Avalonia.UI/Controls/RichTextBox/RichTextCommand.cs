// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Provides the RichTextCommand member.</summary>
internal sealed class RichTextCommand : ICommand
{
    /// <summary>Provides the _execute member.</summary>
    private readonly Action<object?> _execute;

    /// <summary>Provides the documented member.</summary>
    private readonly Predicate<object?>? _canExecute;

    /// <summary>Initializes a new instance of the <see cref="RichTextCommand"/> class.</summary>
    /// <param name="execute">The execute value.</param>
    /// <param name="canExecute">The canExecute value.</param>
    public RichTextCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>Provides the CanExecuteChanged member.</summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>Provides the CanExecute member.</summary>
    /// <param name="parameter">The parameter value.</param>
    /// <returns>The result.</returns>
    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    /// <summary>Provides the Execute member.</summary>
    /// <param name="parameter">The parameter value.</param>
    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }

        _execute(parameter);
    }

    /// <summary>Provides the RaiseCanExecuteChanged member.</summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
