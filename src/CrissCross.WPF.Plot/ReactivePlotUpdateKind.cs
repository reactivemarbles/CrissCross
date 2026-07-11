// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Describes the operation represented by a reactive plot update.</summary>
public enum ReactivePlotUpdateKind
{
    /// <summary>Append new samples to the existing series.</summary>
    Append,

    /// <summary>Replace the existing series contents.</summary>
    Replace,

    /// <summary>Clear the target series without affecting other series.</summary>
    Clear,
}
