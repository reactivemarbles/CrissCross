// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Represents one selectable item in a segmented control.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SegmentItem"/> class.
/// </remarks>
/// <param name="key">The stable segment key.</param>
/// <param name="text">The user-facing segment text.</param>
/// <param name="isEnabled">A value indicating whether the segment can be selected.</param>
/// <param name="icon">Optional icon content or icon key used by platform templates.</param>
public sealed class SegmentItem(string key, string text, bool isEnabled = true, object? icon = null)
{
    /// <summary>Gets the stable segment key.</summary>
    public string Key { get; } = key;

    /// <summary>Gets the user-facing segment text.</summary>
    public string Text { get; } = text;

    /// <summary>Gets a value indicating whether the segment can be selected.</summary>
    public bool IsEnabled { get; } = isEnabled;

    /// <summary>Gets optional icon content or icon key used by platform templates.</summary>
    public object? Icon { get; } = icon;

    /// <summary>Gets a value indicating whether the segment has icon content.</summary>
    public bool HasIcon => Icon is not null;
}
