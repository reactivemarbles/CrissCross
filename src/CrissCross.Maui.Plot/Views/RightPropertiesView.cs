// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Maui.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#else
namespace CrissCross.Maui.Plot;
#endif

/// <summary>Native MAUI editor for the selected plot object's appearance.</summary>
public sealed class RightPropertiesView : ContentView, IDisposable
{
    /// <summary>Identifies the editor view model.</summary>
    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
        nameof(ViewModel),
        typeof(RightPropertiesViewModel),
        typeof(RightPropertiesView),
        propertyChanged: static (source, previous, current) => ((RightPropertiesView)source).ObserveModel());

    /// <summary>Stores the save action.</summary>
    private readonly Button _save = new() { Text = "Save appearance" };

    /// <summary>Stores the observed model.</summary>
    private RightPropertiesViewModel? _observedModel;

    /// <summary>Prevents synchronization loops.</summary>
    private bool _updatingEditors;

    /// <summary>Initializes a new instance of the <see cref="RightPropertiesView"/> class.</summary>
    public RightPropertiesView()
    {
        ItemNameTextBox = new();
        LineWidthNumberBox = new() { Keyboard = Keyboard.Numeric };
        ColorsComboBox = new() { Title = "Color" };
        VisibilityComboBox = new() { Title = "Visibility" };
        VerticalStackLayout layout = new() { new Label { Text = "Selected plot item" } };
        layout.Add(ItemNameTextBox);
        layout.Add(LineWidthNumberBox);
        layout.Add(ColorsComboBox);
        layout.Add(VisibilityComboBox);
        layout.Add(_save);
        Content = layout;
        ItemNameTextBox.TextChanged += EditorChanged;
        LineWidthNumberBox.TextChanged += EditorChanged;
        ColorsComboBox.SelectedIndexChanged += SelectionChanged;
        VisibilityComboBox.SelectedIndexChanged += SelectionChanged;
        Loaded += ViewLoaded;
        Unloaded += ViewUnloaded;
        ViewModel = new();
    }

    /// <summary>Gets or sets the selected appearance view model.</summary>
    public RightPropertiesViewModel? ViewModel
    {
        get => (RightPropertiesViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>Gets the binding root.</summary>
    public RightPropertiesViewModel? BindingRoot => ViewModel;

    /// <summary>Gets the item name entry.</summary>
    public Entry ItemNameTextBox { get; }

    /// <summary>Gets the line width entry.</summary>
    public Entry LineWidthNumberBox { get; }

    /// <summary>Gets the color picker.</summary>
    public Picker ColorsComboBox { get; }

    /// <summary>Gets the visibility picker.</summary>
    public Picker VisibilityComboBox { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        StopObserving();
        Loaded -= ViewLoaded;
        Unloaded -= ViewUnloaded;
        ItemNameTextBox.TextChanged -= EditorChanged;
        LineWidthNumberBox.TextChanged -= EditorChanged;
        ColorsComboBox.SelectedIndexChanged -= SelectionChanged;
        VisibilityComboBox.SelectedIndexChanged -= SelectionChanged;
        ViewModel?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>Observes the selected model without reflection.</summary>
    private void ObserveModel()
    {
        StopObserving();
        _observedModel = ViewModel;
        if (_observedModel is not { } model)
        {
            return;
        }

        model.PropertyChanged += ModelChanged;
        ((INotifyCollectionChanged)model.LineColors).CollectionChanged += ChoicesChanged;
        ((INotifyCollectionChanged)model.Visibilities).CollectionChanged += ChoicesChanged;
        _save.Command = model.SaveConfiguration;
        BindingContext = model;
        UpdateEditors();
    }

    /// <summary>Releases model and collection observations.</summary>
    private void StopObserving()
    {
        if (_observedModel is not { } model)
        {
            return;
        }

        model.PropertyChanged -= ModelChanged;
        ((INotifyCollectionChanged)model.LineColors).CollectionChanged -= ChoicesChanged;
        ((INotifyCollectionChanged)model.Visibilities).CollectionChanged -= ChoicesChanged;
        _observedModel = null;
    }

    /// <summary>Copies model values into native editors.</summary>
    private void UpdateEditors()
    {
        if (_observedModel is not { } model)
        {
            return;
        }

        _updatingEditors = true;
        try
        {
            if (ColorsComboBox.ItemsSource is not string[] colors || !colors.SequenceEqual(model.LineColors))
            {
                ColorsComboBox.ItemsSource = model.LineColors.ToArray();
            }

            if (VisibilityComboBox.ItemsSource is not string[] visibilities || !visibilities.SequenceEqual(model.Visibilities))
            {
                VisibilityComboBox.ItemsSource = model.Visibilities.ToArray();
            }

            ItemNameTextBox.Text = model.ItemName;
            LineWidthNumberBox.Text = model.LineWidth.ToString(CultureInfo.InvariantCulture);
            ColorsComboBox.SelectedItem = model.LineColor;
            VisibilityComboBox.SelectedItem = model.ItemVisibility;
        }
        finally
        {
            _updatingEditors = false;
        }
    }

    /// <summary>Refreshes editors on their native dispatcher.</summary>
    private void RefreshEditors()
    {
        if (Dispatcher.IsDispatchRequired)
        {
            _ = Dispatcher.Dispatch(UpdateEditors);
            return;
        }

        UpdateEditors();
    }

    /// <summary>Applies edited text.</summary>
    /// <param name="sender">The entry.</param>
    /// <param name="args">The new text.</param>
    private void EditorChanged(object? sender, TextChangedEventArgs args)
    {
        if (_updatingEditors || ViewModel is not { } model)
        {
            return;
        }

        if (ReferenceEquals(sender, ItemNameTextBox))
        {
            model.ItemName = ItemNameTextBox.Text;
        }
        else if (double.TryParse(LineWidthNumberBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var width) && double.IsFinite(width) && width >= 0)
        {
            model.LineWidth = width;
        }
    }

    /// <summary>Applies an edited selection.</summary>
    /// <param name="sender">The picker.</param>
    /// <param name="args">The selection event.</param>
    private void SelectionChanged(object? sender, EventArgs args)
    {
        if (_updatingEditors || ViewModel is not { } model)
        {
            return;
        }

        if (ReferenceEquals(sender, ColorsComboBox))
        {
            model.LineColor = ColorsComboBox.SelectedItem as string;
        }
        else
        {
            model.ItemVisibility = VisibilityComboBox.SelectedItem as string;
        }
    }

    /// <summary>Refreshes model changes.</summary>
    /// <param name="sender">The model.</param>
    /// <param name="args">The property event.</param>
    private void ModelChanged(object? sender, PropertyChangedEventArgs args) => RefreshEditors();

    /// <summary>Refreshes available choices.</summary>
    /// <param name="sender">The collection.</param>
    /// <param name="args">The collection event.</param>
    private void ChoicesChanged(object? sender, NotifyCollectionChangedEventArgs args) => Dispatcher.Dispatch(UpdateEditors);

    /// <summary>Reconnects editor observations.</summary>
    /// <param name="sender">The control.</param>
    /// <param name="args">The event.</param>
    private void ViewLoaded(object? sender, EventArgs args) => ObserveModel();

    /// <summary>Disconnects editor observations.</summary>
    /// <param name="sender">The control.</param>
    /// <param name="args">The event.</param>
    private void ViewUnloaded(object? sender, EventArgs args) => StopObserving();
}
