// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents an icon that uses an <see cref="System.Windows.Controls.Image"/> as its content.
/// </summary>
public class ImageIcon : IconElement
{
    /// <summary>
    /// Property for <see cref="Source"/>.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source),
        typeof(ImageSource),
        typeof(ImageIcon),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            OnSourcePropertyChanged));

    /// <summary>
    /// The image.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private
    protected System.Windows.Controls.Image? Image;
#pragma warning restore SA1401 // Fields should be private

    /// <summary>
    /// Gets or sets the Source on this Image.
    /// </summary>
    public ImageSource? Source
    {
        get => (ImageSource)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Initializes the children.
    /// </summary>
    /// <returns>
    /// A UIElement.
    /// </returns>
    protected override UIElement InitializeChildren()
    {
        Image = new System.Windows.Controls.Image() { Source = Source, Stretch = Stretch.UniformToFill };

        return Image;
    }

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
