// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Automation.Peers;
using Avalonia.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Exposes the actual measured paper slices to accessibility.</summary>
public sealed partial class PagedDocumentPresenter
{
    /// <summary>The measured page regions, independent of body positions.</summary>
    private readonly List<AutomationStory> _automationStories = [];

    /// <summary>Creates peers over real repeated-region presenters.</summary>
    /// <param name="editor">The owning editor.</param>
    /// <returns>The measured page stories in physical sheet order.</returns>
    internal IReadOnlyList<AutomationPeer> CreateAutomationStories(RichTextBox editor)
    {
        var peers = new List<AutomationPeer>(_automationStories.Count);
        foreach (var story in _automationStories)
        {
            peers.Add(new RichTextStoryAutomationPeer(editor, story.Region, story.Presenter, story.Kind, story.Page));
        }

        return peers;
    }

    /// <summary>Gets page or formatted line ranges from the current layout.</summary>
    /// <param name="pages">Whether to return paper slices.</param>
    /// <returns>Ranges in document reading order.</returns>
    internal List<(int Start, int End)> GetAutomationUnitRanges(bool pages)
    {
        var result = new List<(int Start, int End)>();
        foreach (var page in _pages)
        {
            if (pages)
            {
                result.Add((page.Slice.Start, page.Slice.End));
                continue;
            }

            foreach (var line in page.Viewport.Presenter.GetAutomationLineRanges())
            {
                var start = Math.Max(line.Start, page.Slice.Start);
                var end = Math.Min(line.End, page.Slice.End);
                if (end > start || (line.Start == line.End && start == end && start >= page.Slice.Start && start < page.Slice.End))
                {
                    result.Add((start, end));
                }
            }
        }

        return result;
    }

    /// <summary>A real page story and its measured visual.</summary>
    /// <param name="Region">The clipping region.</param>
    /// <param name="Presenter">The rendered story.</param>
    /// <param name="Kind">The accessible region kind.</param>
    /// <param name="Page">The physical page number.</param>
    private sealed record AutomationStory(Control Region, FormattedTextPresenter Presenter, string Kind, int Page);
}
