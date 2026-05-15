// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a descriptor-driven property inspector surface.
/// </summary>
public class PropertyGridLite : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="InspectorState"/>.
    /// </summary>
    public static readonly StyledProperty<PropertyGridState?> InspectorStateProperty = AvaloniaProperty.Register<PropertyGridLite, PropertyGridState?>(nameof(InspectorState));

    /// <summary>
    /// Property for <see cref="SearchText"/>.
    /// </summary>
    public static readonly StyledProperty<string?> SearchTextProperty = AvaloniaProperty.Register<PropertyGridLite, string?>(nameof(SearchText));

    /// <summary>
    /// Property for <see cref="CommitChangesCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommitChangesCommandProperty = AvaloniaProperty.Register<PropertyGridLite, ICommand?>(nameof(CommitChangesCommand));

    /// <summary>
    /// Property for <see cref="ResetChangesCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> ResetChangesCommandProperty = AvaloniaProperty.Register<PropertyGridLite, ICommand?>(nameof(ResetChangesCommand));

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGridLite"/> class.
    /// </summary>
    public PropertyGridLite()
    {
        CommitCommand = new InspectorCommand(CommitChanges, () => InspectorState?.CanCommit == true);
        ResetCommand = new InspectorCommand(ResetChanges, () => InspectorState?.CanReset == true);
    }

    /// <summary>
    /// Gets or sets the shared inspector state projected by the control.
    /// </summary>
    public PropertyGridState? InspectorState
    {
        get => GetValue(InspectorStateProperty);
        set => SetValue(InspectorStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the search text used to filter property descriptors.
    /// </summary>
    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when modified properties are committed.
    /// </summary>
    public ICommand? CommitChangesCommand
    {
        get => GetValue(CommitChangesCommandProperty);
        set => SetValue(CommitChangesCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when modified properties are reset.
    /// </summary>
    public ICommand? ResetChangesCommand
    {
        get => GetValue(ResetChangesCommandProperty);
        set => SetValue(ResetChangesCommandProperty, value);
    }

    /// <summary>
    /// Gets the command that commits modified properties.
    /// </summary>
    public ICommand CommitCommand { get; }

    /// <summary>
    /// Gets the command that resets modified properties.
    /// </summary>
    public ICommand ResetCommand { get; }

    /// <summary>
    /// Creates a filtered state snapshot using the current search text.
    /// </summary>
    /// <returns>The filtered state snapshot.</returns>
    public PropertyGridState CreateVisibleState() => InspectorState is null ? new PropertyGridState(searchText: SearchText) : new PropertyGridState(InspectorState.Descriptors, SearchText, InspectorState.IsCommitting);

    /// <summary>
    /// Commits the current inspector state through the configured command hook.
    /// </summary>
    public void CommitChanges()
    {
        var state = CreateVisibleState();
        if (CommitChangesCommand?.CanExecute(state) == true)
        {
            CommitChangesCommand.Execute(state);
        }
    }

    /// <summary>
    /// Resets the current inspector state through the configured command hook.
    /// </summary>
    public void ResetChanges()
    {
        var state = InspectorState;
        if (ResetChangesCommand?.CanExecute(state) == true)
        {
            ResetChangesCommand.Execute(state);
        }
    }

    private sealed class InspectorCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public InspectorCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute();

        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                _execute();
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
