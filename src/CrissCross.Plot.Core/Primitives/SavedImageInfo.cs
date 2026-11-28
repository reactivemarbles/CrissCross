// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot;

/// <summary>Provides information and actions for an image saved by a plot.</summary>
/// <param name="path">The path to the saved image.</param>
/// <param name="fileSize">The size of the saved image in bytes.</param>
public class SavedImageInfo(string path, int fileSize)
{
    /// <summary>The conversion factor from bytes to kilobytes.</summary>
    private const double BytesToKilobytes = 0.001;

    /// <summary>The number of milliseconds in a second.</summary>
    private const int MillisecondsPerSecond = 1000;

    /// <summary>The HTML template used to display the image in a browser.</summary>
    private readonly string _imageHtmlTemplate = """
        <!doctype html>
        <html lang="en">

        <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1">
            <title>CrissCross Plot {{ FILENAME }}</title>
            <style>
                body {
                    padding: 0;
                    margin: 0;
                    text-align: center;
                    background-color: #f0f0f0;
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                }

                img {
                    background-color: white;
                    max-width: 80%;
                    margin: 5em auto 3em auto;
                    display: block;
                    box-shadow: 0px 0px 20px 10px rgba(0, 0, 0, 0.2);
                }

                a {
                    text-decoration: none;
                    color: black;
                }

                a:hover {
                    text-decoration: underline;
                }
            </style>

            <script language="javascript">
                function toggle(){
                    const url = isLive() ? '?' : '?refresh={{ REFRESH_MSEC }}';
                    window.location.href = url;
                }
        
                function isLive(){
                    const urlParams = new URLSearchParams(window.location.search);
                    const refreshMsec = urlParams.get('refresh');
                    return refreshMsec > 0;
                }
            </script>

        </head>

        <body>
            <a href='{{ FILE }}'><img src="{{ FILE }}" width="{{ WIDTH }}" height="{{ HEIGHT }}"></a>
            <div style='line-height: 150%;'>
                <div>{{ FILENAME }} ({{ WIDTH }}x{{ HEIGHT }}) <script>document.write(new Date().toLocaleTimeString());</script></div>
                <div><input id="liveCheckbox" type="checkbox" onclick="toggle();"> auto-refresh</div>
                <div style='opacity: .5;'><a href='https://github.com/reactivemarbles/CrissCross'>{{ VERSION }}</a></div>
            </div>
        </body>

        <script language="javascript">
            document.getElementById('liveCheckbox').checked = isLive();
            const urlParams = new URLSearchParams(window.location.search);
            const refreshMsec = urlParams.get('refresh');
            if (refreshMsec > 0){
                setTimeout(function(){
                    window.location.reload(1);
                }, refreshMsec);
            }
        </script>

        </html>
        """;

    /// <summary>The rendering details associated with this saved image.</summary>
    private RenderDetails _renderDetails;

    /// <summary>Gets the full path to the saved image.</summary>
    public string Path { get; } = System.IO.Path.GetFullPath(path);

    /// <summary>Gets the saved image size in bytes.</summary>
    public int FileSize { get; } = fileSize;

    /// <summary>Returns a summary of the saved image details.</summary>
    /// <returns>A string describing the saved image.</returns>
    public override string ToString() => $"Saved Image [{_renderDetails.FigureRect.Width}x{_renderDetails.FigureRect.Height}] ({FileSize * BytesToKilobytes} kB) {Path}";

    /// <summary>Stores the details used to render the saved image.</summary>
    /// <param name="renderDetails">The rendering details.</param>
    /// <returns>This saved image information.</returns>
    public SavedImageInfo WithRenderDetails(RenderDetails renderDetails)
    {
        _renderDetails = renderDetails;
        return this;
    }

    /// <summary>Launches the saved image using the operating system's default application.</summary>
    public void LaunchFile() => Platform.LaunchFile(Path);

    /// <summary>Writes the saved image path to the console.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1449",
        Justification = "This existing API explicitly writes to standard output; replacing it with logging would change its behavior.")]
    public void ConsoleWritePath() => Console.WriteLine(Path);

    /// <summary>Creates a browser viewer for the saved image and launches it.</summary>
    /// <param name="refresh">The auto-refresh interval in seconds.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional parameter default is retained for source and binary compatibility.")]
    public void LaunchInBrowser(double refresh = 3)
    {
        string html = _imageHtmlTemplate;
        html = html.Replace("{{ REFRESH_MSEC }}", $"{refresh * MillisecondsPerSecond}")
            .Replace("{{ WIDTH }}", $"{_renderDetails.FigureRect.Width}")
            .Replace("{{ HEIGHT }}", $"{_renderDetails.FigureRect.Height}")
            .Replace("{{ VERSION }}", Version.LongString)
            .Replace("{{ FILE }}", Path)
            .Replace("{{ FILENAME }}", System.IO.Path.GetFileName(Path));

        string htmlFilePath = $"{Path}-viewer.html"; // TODO: use a temporary file
        File.WriteAllText(htmlFilePath, html);
        Platform.LaunchFile(htmlFilePath);
    }
}
