// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Provides the HintColorDecorator member.</summary>
/// <param name="storage">The storage value.</param>
internal sealed class HintColorDecorator(IHintColorStateStorage storage) : IColorStateStorage
{
    /// <summary>Gets or sets ColorState.</summary>
    public ColorState ColorState
    {
        get => storage.HintColorState;
        set => storage.HintColorState = value;
    }
}
