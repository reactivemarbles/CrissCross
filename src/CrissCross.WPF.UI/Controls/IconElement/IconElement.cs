// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents the base class for an icon UI element.
/// </summary>
[TypeConverter(typeof(IconElementConverter))]
public abstract class IconElement : FrameworkElement
{
    /// <summary>
    /// Property for <see cref="Foreground"/>.
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(
        typeof(IconElement),
        new FrameworkPropertyMetadata(
            SystemColors.ControlTextBrush,
            FrameworkPropertyMetadataOptions.Inherits,
            static (d, args) => ((IconElement)d).OnForegroundPropertyChanged(args)));

    private Grid? _layoutRoot;

    static IconElement()
    {
        FocusableProperty.OverrideMetadata(typeof(IconElement), new FrameworkPropertyMetadata(false));
        KeyboardNavigation.IsTabStopProperty.OverrideMetadata(
            typeof(IconElement),
            new FrameworkPropertyMetadata(false));
    }

    /// <inheritdoc cref="Control.Foreground"/>
    [Bindable(true)]
    [Category("Appearance")]
    public Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    /// <summary>
    /// Gets the number of visual child elements within this element.
    /// </summary>
    protected override int VisualChildrenCount => 1;

    /// <summary>
    /// Initializes the children.
    /// </summary>
    /// <returns>A UIElement.</returns>
    protected abstract UIElement InitializeChildren();

    /// <summary>
    /// Raises the <see cref="E:ForegroundPropertyChanged" /> event.
    /// </summary>
    /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
    }

    /// <summary>
    /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)" />, and returns a child at the specified index from a collection of child elements.
    /// </summary>
    /// <param name="index">The zero-based index of the requested child element in the collection.</param>
    /// <returns>
    /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">index.</exception>
    protected override Visual GetVisualChild(int index)
    {
        if (index != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        EnsureLayoutRoot();
        return _layoutRoot!;
    }

    /// <summary>
    /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class.
    /// </summary>
    /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
    /// <returns>
    /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
    /// </returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        EnsureLayoutRoot();

        _layoutRoot!.Measure(availableSize);
        return _layoutRoot.DesiredSize;
    }

    /// <summary>
    /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class.
    /// </summary>
    /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
    /// <returns>
    /// The actual size used.
    /// </returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        EnsureLayoutRoot();

        _layoutRoot!.Arrange(new Rect(default, finalSize));
        return finalSize;
    }

    private void EnsureLayoutRoot()
    {
        if (_layoutRoot != null)
        {
            return;
        }

        _layoutRoot = new Grid { Background = Brushes.Transparent, SnapsToDevicePixels = true, };

        _layoutRoot.Children.Add(InitializeChildren());
        AddVisualChild(_layoutRoot);
    }
}
