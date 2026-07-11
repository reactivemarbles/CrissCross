// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Provides the GifFrame member.</summary>
internal sealed class GifFrame : GifBlock
{
    /// <summary>Provides the ImageSeparator member.</summary>
    internal const int ImageSeparator = 0x2C;

    /// <summary>Initializes a new instance of the <see cref="GifFrame"/> class.</summary>
    private GifFrame()
    {
    }

    /// <summary>Gets the Descriptor value.</summary>
    public GifImageDescriptor? Descriptor { get; private set; }

    /// <summary>Gets the LocalColorTable value.</summary>
    public GifColor[]? LocalColorTable { get; private set; }

    /// <summary>Gets the Extensions value.</summary>
    public IList<GifExtension>? Extensions { get; private set; }

    /// <summary>Gets the ImageData value.</summary>
    public GifImageData? ImageData { get; private set; }

    /// <summary>Gets or sets GraphicControl.</summary>
    public GifGraphicControlExtension? GraphicControl { get; set; }

    internal override GifBlockKind Kind => GifBlockKind.GraphicRendering;

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <param name="controlExtensions">The controlExtensions value.</param>
    /// <returns>The result.</returns>
    internal static new async Task<GifFrame> ReadAsync(Stream stream, IEnumerable<GifExtension> controlExtensions)
    {
        var frame = new GifFrame();

        await frame.ReadInternalAsync(stream, controlExtensions).ConfigureAwait(false);

        return frame;
    }

    /// <summary>Provides the ReadInternalAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <param name="controlExtensions">The controlExtensions value.</param>
    /// <returns>The result.</returns>
    private async Task ReadInternalAsync(Stream stream, IEnumerable<GifExtension> controlExtensions)
    {
        // Note: at this point, the Image Separator (0x2C) has already been read
        Descriptor = await GifImageDescriptor.ReadAsync(stream).ConfigureAwait(false);
        if (Descriptor.HasLocalColorTable)
        {
            LocalColorTable = await GifHelpers.ReadColorTableAsync(stream, Descriptor.LocalColorTableSize).ConfigureAwait(false);
        }

        ImageData = await GifImageData.ReadAsync(stream).ConfigureAwait(false);
        Extensions = controlExtensions.ToList().AsReadOnly();
        GraphicControl = Extensions.OfType<GifGraphicControlExtension>().LastOrDefault();
    }
}
