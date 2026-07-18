// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
namespace CrissCross.WPF.UI.Controls.Decoding;
#endif

/// <summary>Provides the GifBlockKind member.</summary>
internal enum GifBlockKind
{
    /// <summary>Represents the Control value.</summary>
    Control,

    /// <summary>Represents the GraphicRendering value.</summary>
    GraphicRendering,

    /// <summary>Represents the SpecialPurpose value.</summary>
    SpecialPurpose,

    /// <summary>Represents the Other value.</summary>
    Other,
}
