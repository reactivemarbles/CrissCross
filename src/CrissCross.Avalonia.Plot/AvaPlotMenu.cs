// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using CrissCross.Plot;
using CrissCross.Plot.IO;
using AvaPixelFormat = Avalonia.Platform.PixelFormat;
using PlotImage = CrissCross.Plot.Image;
using PlotModel = CrissCross.Plot.Plot;
using PlotPixelFormat = CrissCross.Plot.PixelFormat;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#else
namespace CrissCross.Avalonia.Plot;
#endif

/// <summary>Provides context-menu actions for the Avalonia plot control.</summary>
public class AvaPlotMenu : IPlotMenu
{
    /// <summary>The number of color bytes in each image pixel.</summary>
    private const int BytesPerPixel = 4;

    /// <summary>Image file types supported by the save dialog.</summary>
    private static readonly FilePickerFileType[] FilePickerFileTypes =
    [
        new("PNG Files") { Patterns = ["*.png"] },
        new("JPEG Files") { Patterns = ["*.jpg", "*.jpeg"] },
        new("BMP Files") { Patterns = ["*.bmp"] },
        new("WebP Files") { Patterns = ["*.webp"] },
        new("SVG Files") { Patterns = ["*.svg"] },
        new("All Files") { Patterns = ["*"] },
    ];

    /// <summary>The plot control that owns this menu.</summary>
    private readonly AvaPlot _control;

    /// <summary>Initializes a new instance of the <see cref="AvaPlotMenu"/> class.</summary>
    /// <param name="avaPlot">The control that owns the menu.</param>
    public AvaPlotMenu(AvaPlot avaPlot)
    {
        ArgumentNullException.ThrowIfNull(avaPlot);
        _control = avaPlot;
        Reset();
    }

    /// <summary>Gets or sets the default image file name.</summary>
    public string DefaultSaveImageFilename { get; set; } = "Plot.png";

    /// <inheritdoc/>
    public List<ContextMenuItem> ContextMenuItems { get; set; } = [];

    /// <summary>Creates the default context-menu items.</summary>
    /// <returns>The default menu items.</returns>
    public ContextMenuItem[] GetDefaultContextMenuItems()
    {
        ContextMenuItem saveImage = new() { Label = "Save Image", OnInvoke = OpenSaveImageDialog };
        ContextMenuItem copyImage = new() { Label = "Copy to Clipboard", OnInvoke = CopyToClipboard };
        ContextMenuItem autoscale = new() { Label = "Autoscale", OnInvoke = Autoscale };
        return [saveImage, copyImage, autoscale];
    }

    /// <summary>Creates a context menu for a plot.</summary>
    /// <param name="plot">The plot for menu actions.</param>
    /// <returns>The Avalonia context menu.</returns>
    public ContextMenu GetContextMenu(PlotModel plot)
    {
        List<MenuItem> items = [];
        foreach (ContextMenuItem contextMenuItem in ContextMenuItems)
        {
            if (contextMenuItem.IsSeparator)
            {
                items.Add(new() { Header = "-" });
                continue;
            }

            MenuItem menuItem = new() { Header = contextMenuItem.Label };
            menuItem.Click += (_, _) => contextMenuItem.OnInvoke(plot);
            items.Add(menuItem);
        }

        return new() { ItemsSource = items };
    }

    /// <summary>Opens the save dialog for the plot image.</summary>
    /// <param name="plot">The plot to save.</param>
    public void OpenSaveImageDialog(PlotModel plot) => _ = OpenSaveImageDialogAsync(plot);

    /// <summary>Copies the plot image to the system clipboard.</summary>
    /// <param name="plot">The plot to copy.</param>
    public void CopyToClipboard(PlotModel plot) => _ = CopyToClipboardAsync(plot);

    /// <summary>Autoscales the plot and refreshes the control.</summary>
    /// <param name="plot">The plot to autoscale.</param>
    public void Autoscale(PlotModel plot)
    {
        plot.Axes.AutoScale();
        _control.Refresh();
    }

    /// <inheritdoc/>
    public void ShowContextMenu(Pixel pixel)
    {
        PlotModel? plot = _control.GetPlotAtPixel(pixel);
        if (plot is null || ContextMenuItems.Count == 0)
        {
            return;
        }

        ContextMenu contextMenu = GetContextMenu(plot);
        contextMenu.PlacementRect = new(pixel.X, pixel.Y, 1, 1);
        contextMenu.Open(_control);
    }

    /// <inheritdoc/>
    public void Reset()
    {
        Clear();
        ContextMenuItems.AddRange(GetDefaultContextMenuItems());
    }

    /// <inheritdoc/>
    public void Clear() => ContextMenuItems.Clear();

    /// <inheritdoc/>
    public void Add(string label, Action<PlotModel> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        ContextMenuItems.Add(new() { Label = label, OnInvoke = action });
    }

    /// <inheritdoc/>
    public void AddSeparator() => ContextMenuItems.Add(new() { IsSeparator = true });

    /// <summary>Runs the save dialog and writes the selected image.</summary>
    /// <param name="plot">The plot to save.</param>
    /// <returns>A task that completes after the dialog and save operation finish.</returns>
    private async Task OpenSaveImageDialogAsync(PlotModel plot)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(_control);
        if (topLevel is null)
        {
            return;
        }

        FilePickerSaveOptions options = new() { SuggestedFileName = DefaultSaveImageFilename, FileTypeChoices = FilePickerFileTypes };
        IStorageFile? destinationFile = await topLevel.StorageProvider.SaveFilePickerAsync(options);

        string? path = destinationFile?.TryGetLocalPath();
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        PixelSize renderSize = plot.RenderManager.LastRender.FigureRect.Size;
        try
        {
            _ = plot.Save(path, (int)renderSize.Width, (int)renderSize.Height, ImageFormats.FromFilename(path));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or NotSupportedException or ArgumentException)
        {
            Debug.WriteLine(exception);
        }
    }

    /// <summary>Creates a bitmap and writes it to the clipboard.</summary>
    /// <param name="plot">The plot to copy.</param>
    /// <returns>A task that completes after the clipboard operation finishes.</returns>
    private async Task CopyToClipboardAsync(PlotModel plot)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(_control);
        if (topLevel?.Clipboard is not { } clipboard)
        {
            return;
        }

        PixelSize renderSize = plot.RenderManager.LastRender.FigureRect.Size;
        using PlotImage image = plot.GetImage((int)renderSize.Width, (int)renderSize.Height);
        byte[] bytes = image.GetImageBytes(ImageFormat.Bmp);
        AvaPixelFormat pixelFormat = image.PixelFormat == PlotPixelFormat.Bgra8888
            ? AvaPixelFormat.Bgra8888
            : AvaPixelFormat.Rgba8888;
        AlphaFormat alphaFormat = image.AlphaType switch
        {
            AlphaType.Opaque => AlphaFormat.Opaque,
            AlphaType.Premultiplied => AlphaFormat.Premul,
            _ => AlphaFormat.Unpremul,
        };

        IntPtr pixels = Marshal.AllocHGlobal(bytes.Length - BitmapHeader.FileHeaderSize);
        try
        {
            Marshal.Copy(bytes, BitmapHeader.FileHeaderSize, pixels, bytes.Length - BitmapHeader.FileHeaderSize);
            Bitmap bitmap = new(
                pixelFormat,
                alphaFormat,
                pixels,
                new((int)renderSize.Width, (int)renderSize.Height),
                global::Avalonia.Vector.Zero,
                image.Width * BytesPerPixel);
            await clipboard.SetValueAsync(DataFormat.Bitmap, bitmap);
            bitmap.Dispose();
        }
        finally
        {
            Marshal.FreeHGlobal(pixels);
        }
    }
}
