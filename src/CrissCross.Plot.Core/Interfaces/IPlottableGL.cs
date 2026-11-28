// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>This interface is applied to plottables which can be rendered directly on the GPU using an OpenGL shader.</summary>
public interface IPlottableGL : IPlottable
{
    /// <summary>Gets the control used to display this plottable and access its <see cref="GRContext"/> at render time.</summary>
    IPlotControl PlotControl { get; }

    /// <summary>Used to manually synchronize rendering.</summary>
    void GLFinish();

    /// <summary>Store OpenGL atributes.</summary>
    void StoreGLState();

    /// <summary>Restore previously saved OpenGL atributes.</summary>
    void RestoreGLState();
}
