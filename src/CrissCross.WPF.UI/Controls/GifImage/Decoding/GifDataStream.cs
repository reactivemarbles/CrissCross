// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls.Decoding;

/// <summary>Provides the GifDataStream member.</summary>
internal sealed class GifDataStream
{
    /// <summary>Initializes a new instance of the <see cref="GifDataStream"/> class.</summary>
    private GifDataStream()
    {
    }

    /// <summary>Gets the Header value.</summary>
    public GifHeader? Header { get; private set; }

    /// <summary>Gets or sets GlobalColorTable.</summary>
    public GifColor[]? GlobalColorTable { get; set; }

    /// <summary>Gets or sets Frames.</summary>
    public IList<GifFrame>? Frames { get; set; }

    /// <summary>Gets or sets Extensions.</summary>
    public IList<GifExtension>? Extensions { get; set; }

    /// <summary>Gets or sets RepeatCount.</summary>
    public ushort RepeatCount { get; set; }

    /// <summary>Provides the ReadAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    internal static async Task<GifDataStream> ReadAsync(Stream stream)
    {
        var file = new GifDataStream();
        await file.ReadInternalAsync(stream).ConfigureAwait(false);
        return file;
    }

    /// <summary>Provides the ReadInternalAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
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

        RepeatCount = netscapeExtension is not null
            ? GifHelpers.GetRepeatCount(netscapeExtension)
            : (ushort)1;
    }

    /// <summary>Provides the ReadFramesAsync member.</summary>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
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
                    Action processExtension = extension.Kind switch
                    {
                        GifBlockKind.Control => () => controlExtensions.Add(extension),
                        GifBlockKind.SpecialPurpose => () => specialExtensions.Add(extension),
                        GifBlockKind.GraphicRendering or GifBlockKind.Other => static () => { },
                        _ => static () => { }
                    };

                    processExtension();
                }
                else if (block is GifTrailer)
                {
                    break;
                }
            }
            catch (UnknownBlockTypeException) when (frames.Count > 0)
            {
                // Follow the same approach as Firefox:
                // if extraneous data appears between blocks, treat the stream as terminated.
                // https://dxr.mozilla.org/firefox/source/modules/libpr0n/decoders/gif/nsGIFDecoder2.cpp#894-909
                break;
            }
        }

        Frames = frames.AsReadOnly();
        Extensions = specialExtensions.AsReadOnly();
    }
}
