// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
#if REACTIVE_SHIM
using ReactiveUI.Avalonia.Reactive;
#else
using ReactiveUI;
using ReactiveUI.Avalonia;
#endif

#if REACTIVELIST_REACTIVE
using NumberBox = CrissCross.Reactive.Avalonia.UI.Controls.NumberBox;
namespace CrissCross.Reactive.Avalonia.Plot;
#else
using NumberBox = CrissCross.Avalonia.UI.Controls.NumberBox;
namespace CrissCross.Avalonia.Plot;
#endif

/// <summary>Edits the appearance of the selected plot item.</summary>
public partial class RightPropertiesView : ReactiveUserControl<RightPropertiesViewModel>
{
    /// <summary>Stores the model whose editor notifications are subscribed.</summary>
    private RightPropertiesViewModel? _observedModel;

    /// <summary>Prevents model updates while editor values are synchronized.</summary>
    private bool _updatingEditors;

    /// <summary>Initializes a new instance of the <see cref="RightPropertiesView"/> class.</summary>
    public RightPropertiesView()
    {
        InitializeComponent();
        ViewModel = new();
        DataContext = ViewModel;
        SaveBtn.Command = ViewModel.SaveConfiguration;
        textbox1.PropertyChanged += EditorPropertyChanged;
        LineWidth.PropertyChanged += EditorPropertyChanged;
        colorsComboBox.PropertyChanged += EditorPropertyChanged;
        visibilityComboBox.PropertyChanged += EditorPropertyChanged;
        UpdateEditors();
    }

    /// <summary>Gets the binding root view model.</summary>
    public RightPropertiesViewModel? BindingRoot => ViewModel;

    /// <summary>Gets the item name editor.</summary>
    public TextBox ItemNameTextBox => textbox1;

    /// <summary>Gets the line width editor.</summary>
    public NumberBox LineWidthNumberBox => LineWidth;

    /// <summary>Gets the color selector.</summary>
    public ComboBox ColorsComboBox => colorsComboBox;

    /// <summary>Gets the visibility selector.</summary>
    public ComboBox VisibilityComboBox => visibilityComboBox;

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != ViewModelProperty)
        {
            return;
        }

        if (_observedModel is { } previous)
        {
            previous.PropertyChanged -= ModelPropertyChanged;
            ((INotifyCollectionChanged)previous.LineColors).CollectionChanged -= ChoicesCollectionChanged;
            ((INotifyCollectionChanged)previous.Visibilities).CollectionChanged -= ChoicesCollectionChanged;
        }

        _observedModel = ViewModel;
        if (_observedModel is not { } model)
        {
            return;
        }

        model.PropertyChanged += ModelPropertyChanged;
        ((INotifyCollectionChanged)model.LineColors).CollectionChanged += ChoicesCollectionChanged;
        ((INotifyCollectionChanged)model.Visibilities).CollectionChanged += ChoicesCollectionChanged;
        SaveBtn.Command = model.SaveConfiguration;
        DataContext = model;
        UpdateEditors();
    }

    /// <summary>Updates editors when the selected settings change.</summary>
    /// <param name="sender">The view model.</param>
    /// <param name="e">The changed property.</param>
    private void ModelPropertyChanged(object? sender, PropertyChangedEventArgs e) => RefreshEditors();

    /// <summary>Refreshes selector snapshots when the available choices change.</summary>
    /// <param name="sender">The changed choices collection.</param>
    /// <param name="e">The collection change.</param>
    private void ChoicesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => RefreshEditors();

    /// <summary>Refreshes editors on the Avalonia UI dispatcher.</summary>
    private void RefreshEditors()
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Post(UpdateEditors);
            return;
        }

        UpdateEditors();
    }

    /// <summary>Copies strongly typed model values into the editors.</summary>
    private void UpdateEditors()
    {
        if (ViewModel is not { } model)
        {
            return;
        }

        _updatingEditors = true;
        try
        {
            colorsComboBox.ItemsSource = model.LineColors.ToArray();
            visibilityComboBox.ItemsSource = model.Visibilities.ToArray();
            textbox1.Text = model.ItemName;
            LineWidth.Value = (decimal)model.LineWidth;
            colorsComboBox.SelectedItem = model.LineColor;
            visibilityComboBox.SelectedItem = model.ItemVisibility;
        }
        finally
        {
            _updatingEditors = false;
        }
    }

    /// <summary>Applies editor changes without reflection binding.</summary>
    /// <param name="sender">The changed editor.</param>
    /// <param name="e">The changed Avalonia property.</param>
    private void EditorPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_updatingEditors || ViewModel is not { } model)
        {
            return;
        }

        if (e.Property == TextBox.TextProperty)
        {
            model.ItemName = textbox1.Text;
        }
        else if (e.Property == NumericUpDown.ValueProperty)
        {
            model.LineWidth = (double)(LineWidth.Value ?? 0);
        }
        else if (e.Property == ComboBox.SelectedItemProperty)
        {
            ApplySelection(sender, model);
        }
    }

    /// <summary>Applies the strongly typed selection for the changed selector.</summary>
    /// <param name="sender">The selector.</param>
    /// <param name="model">The settings model.</param>
    private void ApplySelection(object? sender, RightPropertiesViewModel model)
    {
        if (ReferenceEquals(sender, colorsComboBox))
        {
            model.LineColor = colorsComboBox.SelectedItem as string;
        }
        else
        {
            model.ItemVisibility = visibilityComboBox.SelectedItem as string;
        }
    }
}
