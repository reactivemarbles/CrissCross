// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using PlotModel = CrissCross.Plot.Plot;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Provides the WPF plot context menu.</summary>
public class WpfPlotMenu : IPlotMenu
{
    /// <summary>The control that owns the menu.</summary>
    private readonly WpfPlotBase _control;

    /// <summary>Initializes a new instance of the <see cref="WpfPlotMenu"/> class.</summary>
    /// <param name="control">The control that owns the menu.</param>
    public WpfPlotMenu(WpfPlotBase control)
    {
        ThrowHelper.ThrowIfNull(control, nameof(control));
        _control = control;
        Reset();
    }

    /// <summary>Gets or sets the default image file name.</summary>
    public string DefaultSaveImageFilename { get; set; } = "Plot.png";

    /// <inheritdoc/>
    public List<ContextMenuItem> ContextMenuItems { get; set; } = new();

    /// <summary>Copies the plot's most recently rendered size to the clipboard.</summary>
    /// <param name="plot">The plot to copy.</param>
    public static void CopyImageToClipboard(PlotModel plot)
    {
        ThrowHelper.ThrowIfNull(plot, nameof(plot));
        PixelSize lastRenderSize = plot.RenderManager.LastRender.FigureRect.Size;
        byte[] bytes = plot.GetImageBytes((int)lastRenderSize.Width, (int)lastRenderSize.Height);
        using MemoryStream stream = new(bytes);
        BitmapImage bitmap = new();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = stream;
        bitmap.EndInit();
        Clipboard.SetImage(bitmap);
    }

    /// <summary>Creates the default context-menu items.</summary>
    /// <returns>The default menu items.</returns>
    public ContextMenuItem[] GetDefaultContextMenuItems()
    {
        ContextMenuItem saveImage = new() { Label = "Save Image", OnInvoke = OpenSaveImageDialog };

        ContextMenuItem copyImage = new() { Label = "Copy to Clipboard", OnInvoke = CopyImageToClipboard };

        ContextMenuItem autoscale = new() { Label = "Autoscale", OnInvoke = Autoscale, };

        ContextMenuItem newWindow = new() { Label = "Open in New Window", OnInvoke = OpenInNewWindow, };

        return [
            saveImage,
            copyImage,
            autoscale,
            newWindow,
        ];
    }

    /// <summary>Creates a context menu for a plot.</summary>
    /// <param name="plot">The plot for menu actions.</param>
    /// <returns>The context menu.</returns>
    public ContextMenu GetContextMenu(PlotModel plot)
    {
        ContextMenu menu = new();

        foreach (ContextMenuItem curr in ContextMenuItems)
        {
            if (curr.IsSeparator)
            {
                _ = menu.Items.Add(new Separator());
            }
            else
            {
                MenuItem menuItem = new() { Header = curr.Label };
                menuItem.Click += (s, e) => curr.OnInvoke(plot);
                _ = menu.Items.Add(menuItem);
            }
        }

        return menu;
    }

    /// <inheritdoc/>
    public void ShowContextMenu(Pixel pixel)
    {
        PlotModel? plot = _control.GetPlotAtPixel(pixel);
        if (plot is null || ContextMenuItems.Count == 0)
        {
            return;
        }

        var menu = GetContextMenu(plot);
        menu.PlacementTarget = _control;
        menu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
        menu.IsOpen = true;
    }

    /// <summary>Prompts for an image file and saves the plot.</summary>
    /// <param name="plot">The plot to save.</param>
    public void OpenSaveImageDialog(PlotModel plot)
    {
        SaveFileDialog dialog = new()
        {
            FileName = DefaultSaveImageFilename,
            Filter = "PNG Files (*.png)|*.png"
                     + "|JPEG Files (*.jpg, *.jpeg)|*.jpg;*.jpeg"
                     + "|BMP Files (*.bmp)|*.bmp"
                     + "|WebP Files (*.webp)|*.webp"
                     + "|SVG Files (*.svg)|*.svg"
                     + "|All files (*.*)|*.*"
        };

        if (dialog.ShowDialog() is not true || string.IsNullOrEmpty(dialog.FileName))
        {
            return;
        }

        ImageFormat format;
        try
        {
            format = ImageFormats.FromFilename(dialog.FileName);
        }
        catch (ArgumentException)
        {
            _ = MessageBox.Show("Unsupported image file format", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            PixelSize lastRenderSize = plot.RenderManager.LastRender.FigureRect.Size;
            _ = plot.Save(dialog.FileName, (int)lastRenderSize.Width, (int)lastRenderSize.Height, format);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            _ = MessageBox.Show($"Image save failed: {exception.Message}", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>Autoscales the plot and refreshes the control.</summary>
    /// <param name="plot">The plot to autoscale.</param>
    public void Autoscale(PlotModel plot)
    {
        plot.Axes.AutoScale();
        _control.Refresh();
    }

    /// <summary>Opens the plot in an interactive window.</summary>
    /// <param name="plot">The plot to display.</param>
    public void OpenInNewWindow(PlotModel plot)
    {
        WpfPlotViewer.Launch(plot, "Interactive Plot");
        _control.Refresh();
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
    [SuppressMessage("Naming", "SST1313:Use camel case parameter names", Justification = "Preserve the existing parameter name for named arguments.")]
    [SuppressMessage("Naming", "SST1318:Match base member parameter names", Justification = "Preserve the existing parameter name for named arguments.")]
    public void Add(string Label, Action<PlotModel> action) => ContextMenuItems.Add(new() { Label = Label, OnInvoke = action });

    /// <inheritdoc/>
    public void AddSeparator() => ContextMenuItems.Add(new() { IsSeparator = true });
}
