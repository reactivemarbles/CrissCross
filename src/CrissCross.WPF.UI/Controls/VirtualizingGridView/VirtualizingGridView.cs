// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

//// Based on VirtualizingWrapPanel created by S. Bäumlisberger licensed under MIT license.
//// https://github.com/sbaeumlisberger/VirtualizingWrapPanel
//// Copyright (C) S. Bäumlisberger
//// All Rights Reserved.

using System.Windows.Controls;
using System.Windows.Data;

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Simple control that displays a gird of items. Depending on the orientation, the items are either stacked horizontally or vertically
/// until the items are wrapped to the next row or column. The control is using virtualization to support large amount of items.
/// <para>In order to work properly all items must have the same size.</para>
/// <para>Based on <see href="https://github.com/sbaeumlisberger/VirtualizingWrapPanel"/>.</para>
/// </summary>
public class VirtualizingGridView : ListView
{
    /// <summary>
    /// Property for <see cref="Orientation"/>.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(VirtualizingGridView),
        new PropertyMetadata(Orientation.Vertical));

    /// <summary>
    /// Property for <see cref="SpacingMode"/>.
    /// </summary>
    public static readonly DependencyProperty SpacingModeProperty = DependencyProperty.Register(
        nameof(SpacingMode),
        typeof(SpacingMode),
        typeof(VirtualizingGridView),
        new PropertyMetadata(SpacingMode.Uniform));

    /// <summary>
    /// Property for <see cref="StretchItems"/>.
    /// </summary>
    public static readonly DependencyProperty StretchItemsProperty = DependencyProperty.Register(
        nameof(StretchItems),
        typeof(bool),
        typeof(VirtualizingGridView),
        new PropertyMetadata(false));

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualizingGridView"/> class.
    /// </summary>
    public VirtualizingGridView()
    {
        VirtualizingPanel.SetCacheLengthUnit(this, VirtualizationCacheLengthUnit.Page);
        VirtualizingPanel.SetCacheLength(this, new VirtualizationCacheLength(1));
        VirtualizingPanel.SetIsVirtualizingWhenGrouping(this, true);
    }

    /// <summary>
    /// Gets or sets a value that specifies the orientation in which items are arranged. The default value is <see cref="Orientation.Vertical"/>.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing mode used when arranging the items. The default value is <see cref="SpacingMode.Uniform"/>.
    /// </summary>
    public SpacingMode SpacingMode
    {
        get => (SpacingMode)GetValue(SpacingModeProperty);
        set => SetValue(SpacingModeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets a value that specifies if the items get stretched to fill up remaining space. The default value is false.
    /// </summary>
    /// <remarks>
    /// The MaxWidth and MaxHeight properties of the ItemContainerStyle can be used to limit the stretching.
    /// In this case the use of the remaining space will be determined by the SpacingMode property.
    /// </remarks>
    public bool StretchItems
    {
        get => (bool)GetValue(StretchItemsProperty);
        set => SetValue(StretchItemsProperty, value);
    }

    /// <summary>
    /// Raises the <see cref="E:Initialized" /> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        InitializeItemsPanel();
    }

    /// <summary>
    /// Initializes the <see cref="ItemsControl.ItemsPanel"/> with <see cref="VirtualizingWrapPanel"/>.
    /// </summary>
    protected virtual void InitializeItemsPanel()
    {
        var factory = new FrameworkElementFactory(typeof(VirtualizingWrapPanel));

        factory.SetBinding(
            VirtualizingWrapPanel.OrientationProperty,
            new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(Orientation)),
                Mode = BindingMode.OneWay
            });
        factory.SetBinding(
            VirtualizingWrapPanel.SpacingModeProperty,
            new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(SpacingMode)),
                Mode = BindingMode.OneWay
            });
        factory.SetBinding(
            VirtualizingWrapPanel.StretchItemsProperty,
            new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(StretchItems)),
                Mode = BindingMode.OneWay
            });

        ItemsPanel = new ItemsPanelTemplate(factory);
    }
}
