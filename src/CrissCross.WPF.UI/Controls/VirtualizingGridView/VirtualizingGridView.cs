// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Data;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>
/// Simple control that displays a gird of items. Depending on the orientation, the items are either stacked
/// horizontally or vertically
/// until the items are wrapped to the next row or column. The control is using virtualization to support large amount
/// of items.
/// <para>In order to work properly all items must have the same size.</para>
/// <para>Based on <see href="https://github.com/sbaeumlisberger/VirtualizingWrapPanel"/>.</para>
/// </summary>
public class VirtualizingGridView : ListView
{
    /// <summary>Property for <see cref="Orientation"/>.</summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(VirtualizingGridView),
        new PropertyMetadata(Orientation.Vertical));

    /// <summary>Property for <see cref="SpacingMode"/>.</summary>
    public static readonly DependencyProperty SpacingModeProperty = DependencyProperty.Register(
        nameof(SpacingMode),
        typeof(SpacingMode),
        typeof(VirtualizingGridView),
        new PropertyMetadata(SpacingMode.Uniform));

    /// <summary>Property for <see cref="StretchItems"/>.</summary>
    public static readonly DependencyProperty StretchItemsProperty = DependencyProperty.Register(
        nameof(StretchItems),
        typeof(bool),
        typeof(VirtualizingGridView),
        new PropertyMetadata(false));

    /// <summary>Gets or sets a value that specifies the orientation in which items are arranged.</summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>Gets or sets the spacing mode used when arranging the items. The default value is Uniform.</summary>
    public SpacingMode SpacingMode
    {
        get => (SpacingMode)GetValue(SpacingModeProperty);
        set => SetValue(SpacingModeProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether gets or sets a value that specifies if the items get stretched
    /// to fill up remaining space. The default value is false.</summary>
    /// <remarks>
    /// The MaxWidth and MaxHeight properties of the ItemContainerStyle can be used to limit the stretching.
    /// In this case the use of the remaining space will be determined by the SpacingMode property.
    /// </remarks>
    public bool StretchItems
    {
        get => (bool)GetValue(StretchItemsProperty);
        set => SetValue(StretchItemsProperty, value);
    }

    /// <summary>Raises the <see cref="E:Initialized" /> event.</summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        VirtualizingPanel.SetCacheLengthUnit(this, VirtualizationCacheLengthUnit.Page);
        VirtualizingPanel.SetCacheLength(this, new VirtualizationCacheLength(1));
        VirtualizingPanel.SetIsVirtualizingWhenGrouping(this, true);
        InitializeItemsPanel();
    }

    /// <summary>Initializes the ItemsPanel with VirtualizingWrapPanel.</summary>
    protected virtual void InitializeItemsPanel()
    {
        var factory = new FrameworkElementFactory(typeof(VirtualizingWrapPanel));

        factory.SetBinding(
            VirtualizingWrapPanel.OrientationProperty,
            new Binding
            {
                Source = this,
                Path = new(nameof(Orientation)),
                Mode = BindingMode.OneWay,
            });
        factory.SetBinding(
            VirtualizingWrapPanel.SpacingModeProperty,
            new Binding
            {
                Source = this,
                Path = new(nameof(SpacingMode)),
                Mode = BindingMode.OneWay,
            });
        factory.SetBinding(
            VirtualizingWrapPanel.StretchItemsProperty,
            new Binding
            {
                Source = this,
                Path = new(nameof(StretchItems)),
                Mode = BindingMode.OneWay,
            });

        ItemsPanel = new(factory);
    }
}
