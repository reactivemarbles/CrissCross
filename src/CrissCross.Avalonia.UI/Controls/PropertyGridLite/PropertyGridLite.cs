// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a descriptor-driven property inspector surface.</summary>
public class PropertyGridLite : TemplatedControl
{
    /// <summary>Property for <see cref="InspectorState"/>.</summary>
    public static readonly StyledProperty<PropertyGridState?> InspectorStateProperty = AvaloniaProperty.Register<
        PropertyGridLite,
        PropertyGridState?
    >(nameof(InspectorState));

    /// <summary>Property for <see cref="SearchText"/>.</summary>
    public static readonly StyledProperty<string?> SearchTextProperty = AvaloniaProperty.Register<
        PropertyGridLite,
        string?
    >(nameof(SearchText));

    /// <summary>Property for <see cref="CommitChangesCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> CommitChangesCommandProperty = AvaloniaProperty.Register<
        PropertyGridLite,
        ICommand?
    >(nameof(CommitChangesCommand));

    /// <summary>Property for <see cref="ResetChangesCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> ResetChangesCommandProperty = AvaloniaProperty.Register<
        PropertyGridLite,
        ICommand?
    >(nameof(ResetChangesCommand));

    /// <summary>Initializes a new instance of the <see cref="PropertyGridLite"/> class.</summary>
    public PropertyGridLite()
    {
        CommitCommand = new InspectorCommand(CommitChanges, () => InspectorState?.CanCommit == true);
        ResetCommand = new InspectorCommand(ResetChanges, () => InspectorState?.CanReset == true);
    }

    /// <summary>Gets or sets the shared inspector state projected by the control.</summary>
    public PropertyGridState? InspectorState
    {
        get => GetValue(InspectorStateProperty);
        set => SetValue(InspectorStateProperty, value);
    }

    /// <summary>Gets or sets the search text used to filter property descriptors.</summary>
    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    /// <summary>Gets or sets the command invoked when modified properties are committed.</summary>
    public ICommand? CommitChangesCommand
    {
        get => GetValue(CommitChangesCommandProperty);
        set => SetValue(CommitChangesCommandProperty, value);
    }

    /// <summary>Gets or sets the command invoked when modified properties are reset.</summary>
    public ICommand? ResetChangesCommand
    {
        get => GetValue(ResetChangesCommandProperty);
        set => SetValue(ResetChangesCommandProperty, value);
    }

    /// <summary>Gets the command that commits modified properties.</summary>
    public ICommand CommitCommand { get; }

    /// <summary>Gets the command that resets modified properties.</summary>
    public ICommand ResetCommand { get; }

    /// <summary>Creates a filtered state snapshot using the current search text.</summary>
    /// <returns>The filtered state snapshot.</returns>
    public PropertyGridState CreateVisibleState() =>
        InspectorState is null
            ? new PropertyGridState(null, SearchText)
            : new PropertyGridState(InspectorState.Descriptors, SearchText, InspectorState.IsCommitting);

    /// <summary>Commits the current inspector state through the configured command hook.</summary>
    public void CommitChanges()
    {
        var state = CreateVisibleState();
        if (CommitChangesCommand?.CanExecute(state) != true)
        {
            return;
        }

        CommitChangesCommand.Execute(state);
    }

    /// <summary>Resets the current inspector state through the configured command hook.</summary>
    public void ResetChanges()
    {
        var state = InspectorState;
        if (ResetChangesCommand?.CanExecute(state) != true)
        {
            return;
        }

        ResetChangesCommand.Execute(state);
    }

    /// <summary>Provides the InspectorCommand member.</summary>
    /// <param name="execute">The execute value.</param>
    /// <param name="canExecute">The canExecute value.</param>
    private sealed class InspectorCommand(Action execute, Func<bool> canExecute) : ICommand
    {
        /// <summary>Provides the _execute member.</summary>
        private readonly Action _execute = execute;

        /// <summary>Provides the documented member.</summary>
        private readonly Func<bool> _canExecute = canExecute;

        /// <summary>Provides the CanExecuteChanged member.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Provides the CanExecute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        /// <returns>The result.</returns>
        public bool CanExecute(object? parameter) => _canExecute();

        /// <summary>Provides the Execute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            _execute();
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
