// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

internal class HintColorDecorator(IHintColorStateStorage storage) : IColorStateStorage
{
    public ColorState ColorState
    {
        get => storage.HintColorState;
        set => storage.HintColorState = value;
    }
}
