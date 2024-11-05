// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;

namespace CrissCross.WPF.UI.Controls.Decoding;

internal sealed class GifDataStream
{
    private GifDataStream()
    {
    }

    public GifHeader? Header { get; private set; }

    public GifColor[]? GlobalColorTable { get; set; }

    public IList<GifFrame>? Frames { get; set; }

    public IList<GifExtension>? Extensions { get; set; }

    public ushort RepeatCount { get; set; }

    internal static async Task<GifDataStream> ReadAsync(Stream stream)
    {
        var file = new GifDataStream();
        await file.ReadInternalAsync(stream).ConfigureAwait(false);
        return file;
    }

    private async Task ReadInternalAsync(Stream stream)
    {
        Header = await GifHeader.ReadAsync(stream).ConfigureAwait(false);

        if (Header.LogicalScreenDescriptor?.HasGlobalColorTable == true)
        {
            GlobalColorTable = await GifHelpers.ReadColorTableAsync(stream, Header.LogicalScreenDescriptor.GlobalColorTableSize).ConfigureAwait(false);
        }

        await ReadFramesAsync(stream).ConfigureAwait(false);

        var netscapeExtension =
                        Extensions?
                            .OfType<GifApplicationExtension>()
                            .FirstOrDefault(GifHelpers.IsNetscapeExtension);

        RepeatCount = netscapeExtension != null
            ? GifHelpers.GetRepeatCount(netscapeExtension)
            : (ushort)1;
    }

    private async Task ReadFramesAsync(Stream stream)
    {
        var frames = new List<GifFrame>();
        var controlExtensions = new List<GifExtension>();
        var specialExtensions = new List<GifExtension>();
        while (true)
        {
            try
            {
                var block = await GifBlock.ReadAsync(stream, controlExtensions).ConfigureAwait(false);

                if (block.Kind == GifBlockKind.GraphicRendering)
                {
                    controlExtensions = [];
                }

                if (block is GifFrame frame)
                {
                    frames.Add(frame);
                }
                else if (block is GifExtension extension)
                {
                    switch (extension.Kind)
                    {
                        case GifBlockKind.Control:
                            controlExtensions.Add(extension);
                            break;
                        case GifBlockKind.SpecialPurpose:
                            specialExtensions.Add(extension);
                            break;

                            // Just discard plain text extensions for now, since we have no use for it
                    }
                }
                else if (block is GifTrailer)
                {
                    break;
                }
            }

            // Follow the same approach as Firefox:
            // If we find extraneous data between blocks, just assume the stream
            // was successfully terminated if we have some successfully decoded frames
            // https://dxr.mozilla.org/firefox/source/modules/libpr0n/decoders/gif/nsGIFDecoder2.cpp#894-909
            catch (UnknownBlockTypeException) when (frames.Count > 0)
            {
                break;
            }
        }

        Frames = frames.AsReadOnly();
        Extensions = specialExtensions.AsReadOnly();
    }
}
