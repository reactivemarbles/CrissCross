// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>ViewModel for the progress controls page.</summary>
public class ProgressPageViewModel : RxObject
{
    /// <summary>Initializes a new instance of the <see cref="ProgressPageViewModel"/> class.</summary>
    public ProgressPageViewModel() =>
        this.BuildComplete(() => DisplayName = "Progress Controls");
}
