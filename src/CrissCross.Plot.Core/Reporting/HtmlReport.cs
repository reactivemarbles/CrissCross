// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Reporting;

/// <summary>Builds an HTML report containing plots and custom content.</summary>
public class HtmlReport() : PlotCollection
{
    /// <summary>The builder that accumulates report content.</summary>
    private readonly StringBuilder _stringBuilder = new();

    /// <summary>Gets or sets the report title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the report description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the default plot width in pixels.</summary>
    public int DefaultPlotWidth { get; set; } = 600;

    /// <summary>Gets or sets the default plot height in pixels.</summary>
    public int DefaultPlotHeight { get; set; } = 400;

    /// <summary>Gets or sets the HTML document template.</summary>
    public string Template { get; set; } =
        """
        <!doctype html>
        <html lang="en">
          <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1">
            <title>{{ TITLE }}</title>
            <meta name="description" content="{{ DESCRIPTION }}">
            <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
                  rel="stylesheet"
                  integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH"
                  crossorigin="anonymous">
          </head>
          <body>
            <main class='container'>{{ CONTENT }}</main>
          </body>
        </html>
        """;

    /// <summary>Adds a plot to the report using the default PNG dimensions.</summary>
    /// <param name="plot">The plot to add.</param>
    /// <param name="title">The plot title.</param>
    /// <param name="description">The plot description.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Optional defaults are retained for source and binary compatibility.")]
    public new void Add(Plot plot, string? title = null, string? description = null) => AddPlotPng(plot, title ?? string.Empty, description ?? string.Empty);

    /// <summary>Adds a heading to the report.</summary>
    /// <param name="text">The heading text.</param>
    /// <param name="level">The HTML heading level.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional heading level is retained for source and binary compatibility.")]
    public void AddHeading(string text, int level = 1) => _ = _stringBuilder.AppendLine($"<h{level}>{text}</h{level}>");

    /// <summary>Adds a paragraph to the report.</summary>
    /// <param name="text">The paragraph text.</param>
    public void AddParagraph(string text) => _ = _stringBuilder.AppendLine($"<p>{text}</p>");

    /// <summary>Adds raw HTML to the report.</summary>
    /// <param name="html">The HTML to add.</param>
    public void AddHtml(string html) => _ = _stringBuilder.AppendLine(html);

    /// <summary>Adds a plot using the default PNG dimensions.</summary>
    /// <param name="plot">The plot to add.</param>
    /// <param name="title">The plot title.</param>
    /// <param name="description">The plot description.</param>
    public void AddPlotPng(Plot plot, string title, string description) => AddPlotPng(plot, title, description, DefaultPlotWidth, DefaultPlotHeight);

    /// <summary>Adds a plot as a PNG using the specified dimensions.</summary>
    /// <param name="plot">The plot to add.</param>
    /// <param name="title">The plot title.</param>
    /// <param name="description">The plot description.</param>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    public void AddPlotPng(Plot plot, string title, string description, int width, int height)
    {
        base.Add(plot, title, description);
        string plotHtml = plot.GetPngHtml(width, height, styleContent: "max-width: 100%");
        _ = _stringBuilder.AppendLine($"<h1>{title}</h1>");
        _ = _stringBuilder.AppendLine($"<p>{description}</p>");
        _ = _stringBuilder.AppendLine($"<div>{plotHtml}</div>");
    }

    /// <summary>Adds a plot as an SVG using the default dimensions.</summary>
    /// <param name="plot">The plot to add.</param>
    /// <param name="title">The plot title.</param>
    /// <param name="description">The plot description.</param>
    public void AddPlotSvg(Plot plot, string title, string description) => AddPlotSvg(plot, title, description, DefaultPlotWidth, DefaultPlotHeight);

    /// <summary>Adds a plot as an SVG using the specified dimensions.</summary>
    /// <param name="plot">The plot to add.</param>
    /// <param name="title">The plot title.</param>
    /// <param name="description">The plot description.</param>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    public void AddPlotSvg(Plot plot, string title, string description, int width, int height)
    {
        base.Add(plot, title, description);
        string plotHtml = plot.GetSvgHtml(width, height);
        _ = _stringBuilder.AppendLine($"<h1>{title}</h1>");
        _ = _stringBuilder.AppendLine($"<p>{description}</p>");
        _ = _stringBuilder.AppendLine($"<div>{plotHtml}</div>");
    }

    /// <summary>Adds a page break to the report.</summary>
    public void AddPageBreak() => _ = _stringBuilder.AppendLine("<hr class='my-5 invisible' style='page-break-after: always;'>");

    /// <summary>Builds and returns the complete HTML document.</summary>
    /// <returns>The report as an HTML document.</returns>
    public string GetHtml() => Template
            .Replace("{{ TITLE }}", Title)
            .Replace("{{ DESCRIPTION }}", Description)
            .Replace("{{ CONTENT }}", _stringBuilder.ToString());

    /// <summary>Saves the report to a file.</summary>
    /// <param name="filePath">The destination file path.</param>
    public void SaveAs(string filePath) => File.WriteAllText(filePath, GetHtml());
}
