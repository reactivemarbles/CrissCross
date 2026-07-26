// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Provides the GifFrame member.</summary>
internal sealed class GifFrame : GifBlock
{
    /// <summary>Provides the ImageSeparator member.</summary>
    internal const int ImageSeparator = 0x2C;

    /// <summary>Initializes a new instance of the <see cref="GifFrame"/> class.</summary>
    private GifFrame() { }

    /// <summary>Gets the Descriptor value.</summary>
    internal GifImageDescriptor? Descriptor { get; private set; }

    /// <summary>Gets the LocalColorTable value.</summary>
    internal GifColor[]? LocalColorTable { get; private set; }

    /// <summary>Gets the Extensions value.</summary>
    internal IList<GifExtension>? Extensions { get; private set; }

    /// <summary>Gets the ImageData value.</summary>
    internal GifImageData? ImageData { get; private set; }

    /// <summary>Gets or sets GraphicControl.</summary>
    internal GifGraphicControlExtension? GraphicControl { get; set; }

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
            LocalColorTable = await GifHelpers
                .ReadColorTableAsync(stream, Descriptor.LocalColorTableSize)
                .ConfigureAwait(false);
        }

        ImageData = await GifImageData.ReadAsync(stream).ConfigureAwait(false);
        List<GifExtension> extensions = [];
        GifGraphicControlExtension? graphicControl = null;
        foreach (var extension in controlExtensions)
        {
            extensions.Add(extension);
            if (extension is GifGraphicControlExtension candidate)
            {
                graphicControl = candidate;
            }
        }

        Extensions = extensions.AsReadOnly();
        GraphicControl = graphicControl;
    }
}
