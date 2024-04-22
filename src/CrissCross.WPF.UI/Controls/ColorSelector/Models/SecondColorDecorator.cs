// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

internal class SecondColorDecorator(ISecondColorStorage storage) : IColorStateStorage
{
    public ColorState ColorState
    {
        get => storage.SecondColorState;
        set => storage.SecondColorState = value;
    }
}
