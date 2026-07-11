// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents an icon that uses an <see cref="System.Windows.Controls.Image"/> as its content.</summary>
public class ImageIcon : IconElement
{
    /// <summary>Property for <see cref="Source"/>.</summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source),
        typeof(ImageSource),
        typeof(ImageIcon),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            OnSourcePropertyChanged));

    /// <summary>Gets or sets the Source on this Image.</summary>
    public ImageSource? Source
    {
        get => (ImageSource)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>Gets or sets the image.</summary>
    protected System.Windows.Controls.Image? Image { get; set; }

    /// <summary>Initializes the children.</summary>
    /// <returns>
    /// A UIElement.
    /// </returns>
    protected override UIElement InitializeChildren()
    {
        var source = Source;
        Image = new() { Source = source, Stretch = Stretch.UniformToFill };

        return Image;
    }

    /// <summary>Provides the OnSourcePropertyChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (ImageIcon)d;
        if (self.Image is null)
        {
            return;
        }

        self.Image.Source = (ImageSource)e.NewValue;
    }
}
