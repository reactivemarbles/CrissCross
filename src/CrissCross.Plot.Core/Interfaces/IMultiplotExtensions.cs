// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides convenience operations for multiplots.</summary>
public static class IMultiplotExtensions
{
    /// <summary>Collapses axes adjacent to the specified edge.</summary>
    /// <param name="plot">The plot whose axes should be collapsed.</param>
    /// <param name="edge">The edge whose axes should be collapsed.</param>
    private static void CollapseAxesAtEdge(Plot plot, Edge edge)
    {
        foreach (IAxis axis in plot.Axes.GetAxes())
        {
            if (axis.Edge == edge)
            {
                axis.Collapse();
            }
        }
    }

    /// <summary>Provides convenience operations for a multiplot.</summary>
    /// <param name="multiplot">The multiplot to operate on.</param>
    extension(IMultiplot multiplot)
    {
        /// <summary>Resets this multiplot so it only contains the given plot.</summary>
        /// <param name="plot">The plot to retain.</param>
        public void Reset(Plot plot)
        {
            multiplot.LastRender.Reset();
            multiplot.AddPlots(0);
            multiplot.AddPlot(plot);
        }

        /// <summary>Resets this multiplot so it only contains the first plot.</summary>
        public void Reset() => multiplot.AddPlots(1);

        /// <summary>Creates an image and renders the multiplot onto it.</summary>
        /// <param name="width">The image width in pixels.</param>
        /// <param name="height">The image height in pixels.</param>
        /// <returns>The rendered multiplot image.</returns>
        public Image Render(int width, int height)
        {
            SKImageInfo imageInfo = new(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            SKSurface surface = SKSurface.Create(imageInfo);
            PixelRect rect = new(0, width, height, 0);
            multiplot.Render(surface.Canvas, rect);
            return new(surface);
        }

        /// <summary>Renders the multiplot into the clip boundary of a surface.</summary>
        /// <param name="surface">The surface to render into.</param>
        public void Render(SKSurface surface) => multiplot.Render(surface.Canvas, surface.Canvas.DeviceClipBounds.ToPixelRect());

        /// <summary>Saves the multiplot as a PNG image file.</summary>
        /// <param name="filename">The destination file path.</param>
        /// <param name="width">The image width in pixels.</param>
        /// <param name="height">The image height in pixels.</param>
        /// <returns>Information about the saved image.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Existing optional dimensions are retained for source and binary compatibility.")]
        public SavedImageInfo SavePng(string filename, int width = 800, int height = 600) => multiplot.Render(width, height).SavePng(filename);

        /// <summary>Creates a plot, adds it to the subplot collection, and returns it.</summary>
        /// <returns>The newly created plot.</returns>
        public Plot AddPlot()
        {
            Plot plot = new();
            multiplot.AddPlot(plot);
            return plot;
        }

        /// <summary>Adds a plot to the subplot collection.</summary>
        /// <param name="plot">The plot to add.</param>
        public void AddPlot(Plot plot) => multiplot.Subplots.Add(plot);

        /// <summary>Adds or removes plots to reach the specified subplot count.</summary>
        /// <param name="total">The desired number of subplots.</param>
        public void AddPlots(int total)
        {
            while (multiplot.Subplots.Count < total)
            {
                _ = multiplot.AddPlot();
            }

            while (multiplot.Subplots.Count > total)
            {
                Plot[] plots = multiplot.GetPlots();
                multiplot.RemovePlot(plots[^1]);
            }
        }

        /// <summary>Gets the plot beneath a pixel according to the last render.</summary>
        /// <param name="pixel">The pixel position to inspect.</param>
        /// <returns>The plot at the pixel, or <see langword="null"/> when none is present.</returns>
        public Plot? GetPlotAtPixel(Pixel pixel) => multiplot.LastRender.GetPlotAtPixel(pixel);

        /// <summary>Gets the number of subplots in this multiplot.</summary>
        /// <returns>The number of subplots.</returns>
        public int Count() => multiplot.Subplots.Count;

        /// <summary>Removes a plot from the subplot collection.</summary>
        /// <param name="plot">The plot to remove.</param>
        public void RemovePlot(Plot plot) => multiplot.Subplots.Remove(plot);

        /// <summary>Gets the plot at the specified index.</summary>
        /// <param name="index">The plot index.</param>
        /// <returns>The plot at the index.</returns>
        public Plot GetPlot(int index) => multiplot.Subplots.GetPlot(index);

        /// <summary>Gets all plots in this multiplot.</summary>
        /// <returns>An array containing all plots.</returns>
        public Plot[] GetPlots() => multiplot.Subplots.GetPlots();

        /// <summary>Removes the gap between vertically stacked subplots.</summary>
        public void CollapseVertically()
        {
            Plot[] plots = multiplot.Subplots.GetPlots();
            for (int i = 0; i < plots.Length; i++)
            {
                Plot plot = plots[i];
                if (i > 0)
                {
                    CollapseAxesAtEdge(plot, Edge.Top);
                }

                if (i < plots.Length - 1)
                {
                    CollapseAxesAtEdge(plot, Edge.Bottom);
                }
            }
        }
    }
}
