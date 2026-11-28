// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity;

/// <summary>Describes a user action used to interact with the plot.</summary>
public interface IUserAction
{
    /// <summary>Gets the input device that performed the action, without its state.</summary>
    string Device { get; }

    /// <summary>Gets a description of the input device and its state.</summary>
    string Description { get; }

    /// <summary>Gets or sets the time when the event happened.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016:DateTime public boundary",
        Justification = "Retained as DateTime to preserve the established public interface contract.")]
    DateTime DateTime { get; set; }
}
