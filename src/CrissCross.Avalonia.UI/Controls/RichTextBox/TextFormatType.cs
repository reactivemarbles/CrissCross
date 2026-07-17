// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Specifies the type of text formatting.</summary>
public enum TextFormatType
{
    /// <summary>Bold formatting.</summary>
    Bold,

    /// <summary>Italic formatting.</summary>
    Italic,

    /// <summary>Underline formatting.</summary>
    Underline,

    /// <summary>Strikethrough formatting.</summary>
    Strikethrough
}
