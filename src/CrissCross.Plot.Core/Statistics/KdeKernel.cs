// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Statistics;

/// <summary>Specifies the kernel function used for a kernel density estimate.</summary>
public enum KdeKernel
{
    /// <summary>Uses the Epanechnikov kernel.</summary>
    Epanechnikov,

    /// <summary>Uses the Gaussian kernel.</summary>
    Gaussian,

    /// <summary>Uses the uniform kernel.</summary>
    Uniform,

    /// <summary>Uses the triangular kernel.</summary>
    Triangular
}
