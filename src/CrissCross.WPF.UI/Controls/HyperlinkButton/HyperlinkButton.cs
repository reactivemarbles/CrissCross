// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Button that opens a URL in a web browser.</summary>
public class HyperlinkButton : Button
{
    /// <summary>Property for <see cref="NavigateUri"/>.</summary>
    public static readonly DependencyProperty NavigateUriProperty = DependencyProperty.Register(
        nameof(NavigateUri),
        typeof(string),
        typeof(HyperlinkButton),
        new PropertyMetadata(string.Empty));

    /// <summary>Gets or sets the URL (or application shortcut) to open.</summary>
    public string NavigateUri
    {
        get => (GetValue(NavigateUriProperty) as string) ?? string.Empty;
        set => SetValue(NavigateUriProperty, value);
    }

    /// <summary>Called when a <see cref="T:System.Windows.Controls.Button" /> is clicked.</summary>
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
                "CrissCross.WPF.UI.HyperlinkButton");

            ProcessStartInfo startInfo = new(new Uri(NavigateUri).AbsoluteUri) { UseShellExecute = true };

            _ = Process.Start(startInfo);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}
