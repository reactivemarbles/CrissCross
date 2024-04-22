// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

internal class HintColorDecorator(IHintColorStateStorage storage) : IColorStateStorage
{
    public ColorState ColorState
    {
        get => storage.HintColorState;
        set => storage.HintColorState = value;
    }
}
