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

    internal override GifBlockKind Kind => GifBlockKind.GraphicRendering;

    internal static GifFrame ReadFrame(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
    {
        var frame = new GifFrame();

        frame.Read(stream, controlExtensions, metadataOnly);

        return frame;
    }

    private void Read(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
    {
        // Note: at this point, the Image Separator (0x2C) has already been read
        Descriptor = GifImageDescriptor.ReadImageDescriptor(stream);
        if (Descriptor.HasLocalColorTable)
        {
            LocalColorTable = GifHelpers.ReadColorTable(stream, Descriptor.LocalColorTableSize);
        }

        ImageData = GifImageData.ReadImageData(stream, metadataOnly);
        Extensions = controlExtensions.ToList().AsReadOnly();
    }
}
