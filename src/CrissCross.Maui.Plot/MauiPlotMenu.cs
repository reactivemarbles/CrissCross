// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using CrissCross.Plot;
using Microsoft.Maui.Controls;
using PlotImageFormat = CrissCross.Plot.ImageFormat;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#else
namespace CrissCross.Maui.Plot;
#endif

/// <summary>Provides plot context-menu actions for the native MAUI control.</summary>
public class MauiPlotMenu : IPlotMenu
{
    /// <summary>The zoom multiplier for the zoom-in action.</summary>
    private const double ZoomInFactor = 1.5;

    /// <summary>The zoom multiplier for the zoom-out action.</summary>
    private const double ZoomOutFactor = 0.5;

    /// <summary>The plot control that owns this menu.</summary>
    private readonly MauiPlot _control;

    /// <summary>The time provider used to generate default image names.</summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>Initializes a new instance of the <see cref="MauiPlotMenu"/> class using the system clock.</summary>
    /// <param name="plot">The plot control that owns this menu.</param>
    public MauiPlotMenu(MauiPlot plot)
        : this(plot, TimeProvider.System)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="MauiPlotMenu"/> class.</summary>
    /// <param name="plot">The plot control that owns this menu.</param>
    /// <param name="timeProvider">The time provider used for default image names.</param>
    public MauiPlotMenu(MauiPlot plot, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(plot);
        ArgumentNullException.ThrowIfNull(timeProvider);
        _control = plot;
        _timeProvider = timeProvider;
        Reset();
    }

    /// <summary>Gets or sets the context-menu items displayed by this menu.</summary>
    public List<ContextMenuItem> ContextMenuItems { get; set; } = [];

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

    /// <inheritdoc/>
    public void ShowContextMenu(Pixel pixel)
    {
        PlotModel? plot = _control.GetPlotAtPixel(pixel);
        if (plot is null || ContextMenuItems.Count == 0)
        {
            return;
        }

        FlyoutBase.SetContextFlyout(_control, GetContextMenu(plot));
    }

    /// <summary>Creates the default context-menu items.</summary>
    /// <returns>The default context-menu items.</returns>
    public ContextMenuItem[] GetDefaultContextMenuItems()
    {
        ContextMenuItem saveImage = new() { Label = "Save Image", OnInvoke = SaveImageDialog };
        ContextMenuItem autoscale = new() { Label = "Autoscale", OnInvoke = Autoscale };
        ContextMenuItem zoomIn = new() { Label = "Zoom In", OnInvoke = ZoomIn };
        ContextMenuItem zoomOut = new() { Label = "Zoom Out", OnInvoke = ZoomOut };

        return [saveImage, new() { IsSeparator = true }, autoscale, zoomIn, zoomOut];
    }

    /// <summary>Creates a MAUI flyout for the specified plot.</summary>
    /// <param name="plot">The plot targeted by menu actions.</param>
    /// <returns>The context-menu flyout.</returns>
    public MenuFlyout GetContextMenu(PlotModel plot)
    {
        MenuFlyout flyout = [];

        foreach (ContextMenuItem item in ContextMenuItems)
        {
            if (item.IsSeparator)
            {
                flyout.Add(new MenuFlyoutSeparator());
            }
            else
            {
                MenuFlyoutItem menuItem = new() { Text = item.Label };
                menuItem.Clicked += (_, _) => item.OnInvoke(plot);
                flyout.Add(menuItem);
            }
        }

        return flyout;
    }

    /// <summary>Autoscales the target plot and refreshes the control.</summary>
    /// <param name="plot">The plot to autoscale.</param>
    public void Autoscale(PlotModel plot)
    {
        plot.Axes.AutoScale();
        _control.Refresh();
    }

    /// <summary>Zooms the target plot in and refreshes the control.</summary>
    /// <param name="plot">The plot to zoom.</param>
    public void ZoomIn(PlotModel plot)
    {
        plot.Axes.Zoom(ZoomInFactor, ZoomInFactor);
        _control.Refresh();
    }

    /// <summary>Zooms the target plot out and refreshes the control.</summary>
    /// <param name="plot">The plot to zoom.</param>
    public void ZoomOut(PlotModel plot)
    {
        plot.Axes.Zoom(ZoomOutFactor, ZoomOutFactor);
        _control.Refresh();
    }

    /// <summary>Starts the asynchronous image-save dialog for a menu action.</summary>
    /// <param name="plot">The plot to save.</param>
    public void SaveImageDialog(PlotModel plot) => _ = SaveImageDialogAsync(plot);

    /// <summary>Prompts for an image format and file name, then saves the plot.</summary>
    /// <param name="plot">The plot to save.</param>
    /// <returns>A task that completes after the dialog and save operation finish.</returns>
    private async Task SaveImageDialogAsync(PlotModel plot)
    {
        Page? page = Application.Current is { Windows.Count: > 0 } application ? application.Windows[0].Page : null;
        if (page is null)
        {
            return;
        }

        (string Extension, PlotImageFormat Format)? selection = await SelectImageFormatAsync(page);
        if (selection is null)
        {
            return;
        }

        await PromptForImageNameAndSaveAsync(plot, page, selection.Value.Extension, selection.Value.Format);

        static async Task<(string Extension, PlotImageFormat Format)?> SelectImageFormatAsync(Page currentPage)
        {
            string[] formats = ["PNG Files .png", "JPEG Files .jpg", "BMP Files .bmp", "WebP Files .webp", "SVG Files .svg"];
            string? selectedFormat = await currentPage.DisplayActionSheetAsync("Select a format", null, null, formats);
            if (string.IsNullOrWhiteSpace(selectedFormat))
            {
                return null;
            }

            string extension = Path.GetExtension(selectedFormat);
            try
            {
                return (extension, ImageFormats.FromFilename(extension));
            }
            catch (ArgumentException exception)
            {
                await currentPage.DisplayAlertAsync("Error", exception.Message, "OK");
                return null;
            }
        }
    }

    /// <summary>Prompts for a file name and saves the selected plot image.</summary>
    /// <param name="plot">The plot to save.</param>
    /// <param name="page">The page that displays dialogs.</param>
    /// <param name="extension">The selected filename extension.</param>
    /// <param name="imageFormat">The selected image format.</param>
    /// <returns>A task that completes after the prompt and save operation finish.</returns>
    private async Task PromptForImageNameAndSaveAsync(PlotModel plot, Page page, string extension, PlotImageFormat imageFormat)
    {
        string defaultName = $"Plot_{_timeProvider.GetLocalNow():yyyyMMdd_HHmmss}";
        string? requestedName = await page.DisplayPromptAsync("File name", "Enter the file name", placeholder: defaultName);
        if (requestedName is null or "Cancel")
        {
            return;
        }

        string requestedFileName = string.IsNullOrWhiteSpace(requestedName) ? defaultName : Path.GetFileName(requestedName);
        string fileName = Path.ChangeExtension(requestedFileName, extension);
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string filePath = Path.Combine(folder, fileName);
        PixelSize renderSize = plot.RenderManager.LastRender.FigureRect.Size;

        try
        {
            _ = Directory.CreateDirectory(folder);
            _ = plot.Save(filePath, (int)renderSize.Width, (int)renderSize.Height, imageFormat);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or NotSupportedException or ArgumentException)
        {
            await page.DisplayAlertAsync("Error", $"Image save failed: {exception.Message}", "OK");
            return;
        }

        await page.DisplayAlertAsync("Success", $"Image saved to {filePath}", "OK");
    }
}
