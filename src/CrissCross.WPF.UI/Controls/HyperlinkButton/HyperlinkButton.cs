// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Button that opens a URL in a web browser.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class HyperlinkButton : Button
{
    /// <summary>Property for <see cref="NavigateUri"/>.</summary>
    public static readonly DependencyProperty NavigateUriProperty = DependencyProperty.Register(
        nameof(NavigateUri),
        typeof(string),
        typeof(HyperlinkButton),
        new(string.Empty));

    /// <summary>Gets or sets the URL (or application shortcut) to open.</summary>
    public string NavigateUri
    {
        get => (GetValue(NavigateUriProperty) as string) ?? string.Empty;
        set => SetValue(NavigateUriProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

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
