// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Button that opens a URL in a web browser.
/// </summary>
public class HyperlinkButton : global::Avalonia.Controls.Button
{
    /// <summary>
    /// Property for <see cref="NavigateUri"/>.
    /// </summary>
    public static readonly StyledProperty<string> NavigateUriProperty = AvaloniaProperty.Register<HyperlinkButton, string>(
        nameof(NavigateUri), string.Empty);

    /// <summary>
    /// Gets or sets the URL (or application shortcut) to open.
    /// </summary>
    public string NavigateUri
    {
        get => GetValue(NavigateUriProperty);
        set => SetValue(NavigateUriProperty, value);
    }

    /// <summary>
    /// Called when a <see cref="T:Avalonia.Controls.Button" /> is clicked.
    /// </summary>
    protected override void OnClick()
    {
        base.OnClick();
        if (string.IsNullOrEmpty(NavigateUri))
        {
            return;
        }

        try
        {
            Debug.WriteLine(
                $"INFO | HyperlinkButton clicked, with href: {NavigateUri}",
                "CrissCross.Avalonia.UI.HyperlinkButton");

            ProcessStartInfo sInfo = new(new Uri(NavigateUri).AbsoluteUri) { UseShellExecute = true };

            Process.Start(sInfo);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}
