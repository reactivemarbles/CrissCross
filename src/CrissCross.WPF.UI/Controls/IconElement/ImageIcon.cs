// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents an icon that uses an <see cref="System.Windows.Controls.Image"/> as its content.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
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

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

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
