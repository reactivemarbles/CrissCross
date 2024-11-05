// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;

namespace CrissCross.WPF.UI.Controls.Decoding;

internal sealed class GifFrame : GifBlock
{
    internal const int ImageSeparator = 0x2C;

    private GifFrame()
    {
    }

    public GifImageDescriptor? Descriptor { get; private set; }

    public GifColor[]? LocalColorTable { get; private set; }

    public IList<GifExtension>? Extensions { get; private set; }

    public GifImageData? ImageData { get; private set; }

    public GifGraphicControlExtension? GraphicControl { get; set; }

    internal override GifBlockKind Kind => GifBlockKind.GraphicRendering;

    internal static new async Task<GifFrame> ReadAsync(Stream stream, IEnumerable<GifExtension> controlExtensions)
    {
        var frame = new GifFrame();

        await frame.ReadInternalAsync(stream, controlExtensions).ConfigureAwait(false);

        return frame;
    }

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
